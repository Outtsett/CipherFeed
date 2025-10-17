/*
 * ============================================================================
 * SESSION MANAGER
 * ============================================================================
 * 
 * Manages trading session detection and boundary tracking for RTH (Regular
 * Trading Hours) and ETH (Extended Trading Hours). Handles automatic session
 * transitions and state resets.
 * 
 * SESSION TIMES (CME Futures):
 *   RTH: 4:00 AM - 1:45 PM PST  =  12:00 - 21:45 UTC
 *   ETH: 3:15 PM - 4:00 AM PST  =  23:15 UTC - 12:00 UTC (next day)
 * 
 * ============================================================================
 * QUANTOWER API REFERENCES
 * ============================================================================
 * 
 * Time Management:
 *   - Core.TimeUtils: quantower-api\Core\TimeUtils.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Core.html
 *     Property: Core.Instance.TimeUtils.DateTimeUtcNow
 *     Purpose: Get current UTC time synchronized with platform
 *     Note: Always use UTC for session logic to avoid timezone issues
 * 
 * Market Data Timestamps:
 *   - Last.Time: quantower-api\Core\Quotes\Last.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Last.html
 *     Property: Time (DateTime) - tick timestamp in UTC
 *     Purpose: Used in CheckBoundary() to detect session transitions
 * 
 *   - Quote.Time: quantower-api\Core\Quotes\Quote.md
 *     URL: https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Quote.html
 *     Property: Time (DateTime) - quote timestamp in UTC
 *     Purpose: Alternative timestamp source for session detection
 * 
 * ============================================================================
 * SESSION DETECTION LOGIC
 * ============================================================================
 * 
 * Timeline (UTC):
 *   00:00 ??? ETH (overnight continuation)
 *   12:00 ??? RTH starts ??? Session Transition #1
 *   21:45 ??  RTH ends
 *   23:15 ??? ETH starts ??? Session Transition #2
 *   24:00 ??? ETH continues to next day
 * 
 * Session Boundaries:
 *   ETH ? RTH (12:00 UTC):
 *     ? Reset cumulative delta, volume, snapshots
 *     ? Reinitialize indicators from 12:00 UTC
 *     ? Create new CSV files with RTH timestamp
 *     ? Fire SessionChanged event
 * 
 *   RTH ? ETH (23:15 UTC):
 *     ? Reset cumulative delta, volume, snapshots
 *     ? Reinitialize indicators from 23:15 UTC
 *     ? Create new CSV files with ETH timestamp
 *     ? Fire SessionChanged event
 * 
 * Gap Handling:
 *   21:45 - 23:15 UTC = 1.5 hour gap (no trading)
 *   During gap: LastCheckTime updates but no session change fires
 * 
 * ============================================================================
 * USAGE IN CIPHERFEED STRATEGY
 * ============================================================================
 * 
 * Created in: CipherFeed.cs::OnCreated()
 * Initialized in: CipherFeed.cs::OnRun()
 * Checked on every tick: CipherFeed.cs::OnNewLast_ForSymbol()
 * Event handled by: CipherFeed.cs::OnSessionChanged()
 * 
 * When SessionChanged fires:
 *   1. All dictionaries cleared (sessionOpenPrices, latestSnapshots)
 *   2. All indicators removed and recreated with new historical data
 *   3. CSV exporter recreated with new session timestamp
 *   4. All snapshots reset cumulative state
 * 
 * ============================================================================
 */

using System;

namespace CipherFeed.Core
{
    /// <summary>
    /// Trading session type for CME futures
    /// </summary>
    public enum TradingSession
    {
        /// <summary>Regular Trading Hours (4:00 AM - 1:45 PM PST / 12:00 - 21:45 UTC)</summary>
        RTH,
        
        /// <summary>Extended Trading Hours (3:15 PM - 4:00 AM PST / 23:15 UTC - 12:00 UTC next day)</summary>
        ETH
    }

    /// <summary>
    /// Manages trading session detection and boundaries.
    /// Handles RTH (Regular Trading Hours) and ETH (Extended Trading Hours) transitions.
    /// </summary>
    public class SessionManager
    {
        #region Constants

        // Session times: PST to UTC conversion (PST + 8 hours = UTC)
        // RTH: 4:00 AM - 1:45 PM PST = 12:00 - 21:45 UTC
        // ETH: 3:15 PM - 4:00 AM PST = 23:15 UTC - 12:00 UTC (next day)
        private static readonly TimeSpan RTH_START = new(12, 0, 0);   // 12:00 UTC (4am PST)
        private static readonly TimeSpan RTH_END = new(21, 45, 0);    // 21:45 UTC (1:45pm PST)
        private static readonly TimeSpan ETH_START = new(23, 15, 0);  // 23:15 UTC (3:15pm PST)
        private static readonly TimeSpan ETH_END = new(12, 0, 0);     // 12:00 UTC (4am PST next day)

        #endregion

        #region Events

        /// <summary>
        /// Fired when session changes (RTH ? ETH).
        /// Args: (oldSession, newSession, newSessionStartTime)
        /// </summary>
        public event Action<TradingSession, TradingSession, DateTime> SessionChanged;

        #endregion

        #region Private Fields


        #endregion

        #region Public Properties

        /// <summary>
        /// Current active trading session (RTH or ETH)
        /// </summary>
        public TradingSession CurrentSession { get; private set; }

        /// <summary>
        /// Start time of the current session (UTC)
        /// </summary>
        public DateTime SessionStartTime { get; private set; }

        /// <summary>
        /// Last time a session boundary check was performed (UTC)
        /// </summary>
        public DateTime LastCheckTime { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize session manager with current time
        /// </summary>
        /// <param name="utcNow">Current UTC time</param>
        public void Initialize(DateTime utcNow)
        {
            CurrentSession = GetSession(utcNow);
            SessionStartTime = GetSessionStart(utcNow, CurrentSession);
            LastCheckTime = utcNow;
        }

        /// <summary>
        /// Check if session has changed and fire event if it has.
        /// Call this on every tick to detect session boundaries.
        /// </summary>
        /// <param name="utcTime">Current tick time (UTC)</param>
        /// <returns>True if session changed, false otherwise</returns>
        public bool CheckBoundary(DateTime utcTime)
        {
            TradingSession newSession = GetSession(utcTime);

            if (newSession != CurrentSession)
            {
                DateTime newSessionStart = GetSessionStart(utcTime, newSession);
                TradingSession oldSession = CurrentSession;

                // Update state
                CurrentSession = newSession;
                SessionStartTime = newSessionStart;
                LastCheckTime = utcTime;

                // Fire event
                SessionChanged?.Invoke(oldSession, newSession, newSessionStart);

                return true;
            }

            LastCheckTime = utcTime;
            return false;
        }

        /// <summary>
        /// Get trading session for a specific UTC time
        /// </summary>
        /// <param name="utcTime">UTC time to check</param>
        /// <returns>RTH or ETH</returns>
        public TradingSession GetSession(DateTime utcTime)
        {
            TimeSpan time = utcTime.TimeOfDay;

            // RTH: 12:00 - 21:45 UTC
            if (time >= RTH_START && time < RTH_END)
            {
                return TradingSession.RTH;
            }

            // ETH: 23:15 UTC - 12:00 UTC (next day)
            // Covers both evening (23:15-23:59) and overnight (00:00-12:00)
            if (time >= ETH_START || time < ETH_END)
            {
                return TradingSession.ETH;
            }

            // Default to RTH for any edge cases
            return TradingSession.RTH;
        }

        /// <summary>
        /// Get the start time of a session on a specific date
        /// </summary>
        /// <param name="utcTime">Reference UTC time</param>
        /// <param name="session">Session to get start time for</param>
        /// <returns>Session start time in UTC</returns>
        public DateTime GetSessionStart(DateTime utcTime, TradingSession session)
        {
            if (session == TradingSession.RTH)
            {
                // RTH starts at 12:00 UTC
                DateTime rthStart = utcTime.Date.Add(RTH_START);

                // If current time is before RTH start, we're looking at the previous day's session
                return utcTime.TimeOfDay < RTH_START ? rthStart.AddDays(-1) : rthStart;
            }
            else // ETH
            {
                // ETH starts at 23:15 UTC
                // If we're before 12:00 (ETH_END), we started yesterday at 23:15
                // If we're after 23:15, we started today at 23:15
                return utcTime.TimeOfDay < ETH_END
                    ? utcTime.Date.AddDays(-1).Add(ETH_START)
                    : utcTime.Date.Add(ETH_START);
            }
        }

        /// <summary>
        /// Check if a given time is during Regular Trading Hours
        /// </summary>
        public bool IsRTH(DateTime utcTime)
        {
            return GetSession(utcTime) == TradingSession.RTH;
        }

        /// <summary>
        /// Check if a given time is during Extended Trading Hours
        /// </summary>
        public bool IsETH(DateTime utcTime)
        {
            return GetSession(utcTime) == TradingSession.ETH;
        }

        /// <summary>
        /// Get time remaining in current session
        /// </summary>
        /// <param name="utcTime">Current UTC time</param>
        /// <returns>TimeSpan until session end</returns>
        public TimeSpan GetTimeUntilSessionEnd(DateTime utcTime)
        {
            TimeSpan time = utcTime.TimeOfDay;

            if (CurrentSession == TradingSession.RTH)
            {
                // RTH ends at 21:45 UTC
                return time < RTH_END ? RTH_END - time : TimeSpan.Zero;
            }
            else // ETH
            {
                // ETH ends at 12:00 UTC (next day if we're past midnight)
                if (time >= ETH_START)
                {
                    // Evening ETH: time until midnight + time from midnight to ETH_END
                    return TimeSpan.FromHours(24) - time + ETH_END;
                }
                else if (time < ETH_END)
                {
                    // Overnight ETH: time until ETH_END
                    return ETH_END - time;
                }
                return TimeSpan.Zero;
            }
        }

        #endregion

        /*
         * ???????????????????????????????????????????????????????????????????????????????
         * SESSION DETECTION LOGIC
         * ???????????????????????????????????????????????????????????????????????????????
         * 
         * RTH (Regular Trading Hours):
         *   - 4:00 AM - 1:45 PM PST
         *   - 12:00 - 21:45 UTC
         * 
         * ETH (Extended Trading Hours):
         *   - 3:15 PM - 4:00 AM PST (next day)
         *   - 23:15 UTC - 12:00 UTC (next day)
         * 
         * TIMELINE (UTC):
         * ??????????????????????????????????????????????????????????????????????????????
         * 00:00 ?????? ETH (overnight)
         * 12:00 ?????? RTH starts
         * 21:45 ?????? RTH ends (gap until ETH)
         * 23:15 ?????? ETH starts (evening)
         * 24:00 ?????? ETH continues (overnight)
         * 
         * SESSION TRANSITIONS:
         * ??????????????????????????????????????????????????????????????????????????????
         * ETH ? RTH: At 12:00 UTC
         *   - Reset all cumulative state
         *   - Reinitialize indicators from 12:00 UTC
         *   - Create new CSV files
         * 
         * RTH ? ETH: At 23:15 UTC (after 1.5 hour gap)
         *   - Reset all cumulative state
         *   - Reinitialize indicators from 23:15 UTC
         *   - Create new CSV files
         * 
         * ???????????????????????????????????????????????????????????????????????????????
         */
    }
}
