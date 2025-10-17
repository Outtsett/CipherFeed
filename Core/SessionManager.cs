using System;
using TradingPlatform.BusinessLayer;

namespace CipherFeed.Core
{
    /// <summary>
    /// Trading session type for CME futures.
    /// Custom enum (not from Quantower API) for RTH/ETH distinction.
    /// </summary>
    public enum TradingSession
    {
        /// <summary>Regular Trading Hours (4:00 AM - 1:45 PM PST / 12:00 - 21:45 UTC)</summary>
        RTH,

        /// <summary>Extended Trading Hours (3:15 PM - 4:00 AM PST / 23:15 UTC - 12:00 UTC next day)</summary>
        ETH
    }

    /// <summary>
    /// Manages CME futures session detection and boundaries (RTH/ETH).
    /// Uses custom time-based logic with Pacific Standard Time (PST = UTC-8).
    /// 
    /// SESSIONS (PST):
    ///   RTH: 4:00 AM - 1:45 PM PST (12:00 - 21:45 UTC)
    ///   ETH: 3:15 PM - 4:00 AM PST (23:15 UTC - 12:00 UTC next day)
    /// </summary>
    public class SessionManager
    {
        #region Constants

        // Session times in UTC (Pacific Standard Time = UTC-8)
        // RTH: 4:00 AM - 1:45 PM PST
        private static readonly TimeSpan RTH_START = new(12, 0, 0);   // 12:00 UTC = 4:00 AM PST
        private static readonly TimeSpan RTH_END = new(21, 45, 0);    // 21:45 UTC = 1:45 PM PST

        // ETH: 3:15 PM PST - 4:00 AM PST (next day)
        private static readonly TimeSpan ETH_START = new(23, 15, 0);  // 23:15 UTC = 3:15 PM PST
        private static readonly TimeSpan ETH_END = new(12, 0, 0);     // 12:00 UTC = 4:00 AM PST (next day)

        #endregion

        #region Events

        /// <summary>
        /// Fired when session changes (RTH ↔ ETH).
        /// Args: (oldSession, newSession, newSessionStartTime)
        /// 
        /// Subscribers should:
        /// 1. Clear cumulative state (volumes, deltas, snapshots)
        /// 2. Reinitialize indicators with new session start time
        /// 3. Create new CSV files for new session
        /// 4. Reset MarketDataSnapshot.ResetCumulativeState()
        /// </summary>
        public event Action<TradingSession, TradingSession, DateTime> SessionChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Current active trading session (RTH or ETH).
        /// Updated by CheckBoundary() when session transitions occur.
        /// </summary>
        public TradingSession CurrentSession { get; private set; }

        /// <summary>
        /// Start time of the current session (UTC).
        /// Used for historical data requests and indicator initialization.
        /// </summary>
        public DateTime SessionStartTime { get; private set; }

        /// <summary>
        /// Last time a session boundary check was performed (UTC).
        /// Updated on every CheckBoundary() call (typically every tick).
        /// </summary>
        public DateTime LastCheckTime { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize session manager with current UTC time.
        /// Call once in Strategy.OnRun() with Core.Instance.TimeUtils.DateTimeUtcNow
        /// </summary>
        /// <param name="utcNow">Current UTC time from Core.Instance.TimeUtils.DateTimeUtcNow</param>
        public void Initialize(DateTime utcNow)
        {
            CurrentSession = GetSession(utcNow);
            SessionStartTime = GetSessionStart(utcNow, CurrentSession);
            LastCheckTime = utcNow;
        }

        /// <summary>
        /// Check if session has changed and fire SessionChanged event if it has.
        /// Call this on every tick (in OnNewLast handler) to detect session boundaries.
        /// </summary>
        /// <param name="utcTime">Current tick time (UTC) from Last.Time or Quote.Time</param>
        /// <returns>True if session changed, false otherwise</returns>
        public bool CheckBoundary(DateTime utcTime)
        {
            TradingSession newSession = GetSession(utcTime);

            if (newSession != CurrentSession)
            {
                DateTime newSessionStart = GetSessionStart(utcTime, newSession);
                TradingSession oldSession = CurrentSession;

                // Update state BEFORE firing event
                CurrentSession = newSession;
                SessionStartTime = newSessionStart;
                LastCheckTime = utcTime;

                // Fire event to notify subscribers (CipherFeed.OnSessionChanged)
                SessionChanged?.Invoke(oldSession, newSession, newSessionStart);

                return true;
            }

            LastCheckTime = utcTime;
            return false;
        }

        /// <summary>
        /// Get trading session for a specific UTC time.
        /// Pure function - does not modify state.
        /// 
        /// Logic:
        ///   RTH: 12:00 - 21:45 UTC (4:00 AM - 1:45 PM PST)
        ///   ETH: 23:15 UTC - 12:00 UTC next day (3:15 PM - 4:00 AM PST)
        ///   Gap: 21:45 - 23:15 UTC (returns RTH, will transition to ETH at 23:15)
        /// </summary>
        /// <param name="utcTime">UTC time to check</param>
        /// <returns>RTH or ETH</returns>
        public TradingSession GetSession(DateTime utcTime)
        {
            TimeSpan time = utcTime.TimeOfDay;

            // RTH: 12:00 - 21:45 UTC (4:00 AM - 1:45 PM PST)
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

            // Gap period (21:45 - 23:15 UTC): Return RTH
            // This ensures a defined state during the 1.5-hour gap
            // Session will transition to ETH at 23:15 UTC
            return TradingSession.RTH;
        }

        /// <summary>
        /// Get the start time of a session on a specific date.
        /// Used for historical data requests and indicator initialization.
        /// </summary>
        /// <param name="utcTime">Reference UTC time</param>
        /// <param name="session">Session to get start time for</param>
        /// <returns>Session start time in UTC</returns>
        public DateTime GetSessionStart(DateTime utcTime, TradingSession session)
        {
            if (session == TradingSession.RTH)
            {
                // RTH starts at 12:00 UTC (4:00 AM PST)
                DateTime rthStart = utcTime.Date.Add(RTH_START);

                // If current time is before RTH start, we're looking at the previous day's session
                return utcTime.TimeOfDay < RTH_START ? rthStart.AddDays(-1) : rthStart;
            }
            else // ETH
            {
                // ETH starts at 23:15 UTC (3:15 PM PST)
                // If we're before 12:00 (ETH_END), we started yesterday at 23:15
                // If we're after 23:15, we started today at 23:15
                return utcTime.TimeOfDay < ETH_END
                    ? utcTime.Date.AddDays(-1).Add(ETH_START)
                    : utcTime.Date.Add(ETH_START);
            }
        }

        /// <summary>
        /// Get session open price from historical data.
        /// Searches for the first bar at or after the session start time within a ±5 minute window.
        /// </summary>
        /// <param name="symbol">Symbol to get session open for</param>
        /// <param name="message">Output message describing the result (success or reason for failure)</param>
        /// <returns>Session open price if found, null otherwise</returns>
        public double? GetSessionOpenPrice(Symbol symbol, out string message)
        {
            try
            {
                // Request historical data from 5 minutes before session start to 5 minutes after
                HistoryRequestParameters historyParams = new()
                {
                    Symbol = symbol,
                    FromTime = SessionStartTime.AddMinutes(-5),
                    ToTime = SessionStartTime.AddMinutes(5),
                    Aggregation = new HistoryAggregationTime(Period.MIN1, symbol.HistoryType)
                };

                HistoricalData history = symbol.GetHistory(historyParams);

                if (history != null && history.Count > 0)
                {
                    IHistoryItem sessionBar = null;
                    double closestTimeDiff = double.MaxValue;

                    // Find the bar closest to (but not before) the session start time
                    for (int i = 0; i < history.Count; i++)
                    {
                        IHistoryItem bar = history[i, SeekOriginHistory.Begin];
                        if (bar is HistoryItemBar barItem && barItem.TimeLeft >= SessionStartTime)
                        {
                            double timeDiff = (barItem.TimeLeft - SessionStartTime).TotalSeconds;

                            if (timeDiff < closestTimeDiff)
                            {
                                sessionBar = bar;
                                closestTimeDiff = timeDiff;
                            }
                        }
                    }

                    if (sessionBar is HistoryItemBar sessionBarItem)
                    {
                        message = $"Session open from history: {sessionBarItem.Open:F2} (bar time: {sessionBarItem.TimeLeft:yyyy-MM-dd HH:mm})";
                        return sessionBarItem.Open;
                    }
                }

                message = "No historical data found, will use first tick";
                return null;
            }
            catch (Exception ex)
            {
                message = $"Error getting historical data: {ex.Message}, will use first tick";
                return null;
            }
        }

        /// <summary>
        /// Initialize session open price for a symbol.
        /// Tries historical data first, returns null if not found (caller uses first tick as fallback).
        /// </summary>
        /// <param name="symbol">Symbol to initialize session open for</param>
        /// <param name="sessionOpenPrice">Output: session open price if found</param>
        /// <param name="logMessage">Output: message for logging</param>
        /// <returns>True if session open price found from historical data, false otherwise</returns>
        public bool TryInitializeSessionOpen(Symbol symbol, out double sessionOpenPrice, out string logMessage)
        {
            double? price = GetSessionOpenPrice(symbol, out string message);

            if (price.HasValue)
            {
                sessionOpenPrice = price.Value;
                logMessage = $"[{CurrentSession}] {symbol.Name} {message}";
                return true;
            }
            else
            {
                sessionOpenPrice = 0.0;
                logMessage = $"[{CurrentSession}] {symbol.Name} - {message}";
                return false;
            }
        }

        /// <summary>
        /// Check if a given time is during Regular Trading Hours.
        /// Utility method for filtering/conditional logic.
        /// </summary>
        /// <param name="utcTime">UTC time to check</param>
        /// <returns>True if RTH, false otherwise</returns>
        public bool IsRTH(DateTime utcTime)
        {
            return GetSession(utcTime) == TradingSession.RTH;
        }

        /// <summary>
        /// Check if a given time is during Extended Trading Hours.
        /// Utility method for filtering/conditional logic.
        /// </summary>
        /// <param name="utcTime">UTC time to check</param>
        /// <returns>True if ETH, false otherwise</returns>
        public bool IsETH(DateTime utcTime)
        {
            return GetSession(utcTime) == TradingSession.ETH;
        }

        /// <summary>
        /// Get time remaining until current session ends.
        /// Useful for pre-session-end warnings or statistics.
        /// </summary>
        /// <param name="utcTime">Current UTC time</param>
        /// <returns>TimeSpan until session end (or TimeSpan.Zero if past end)</returns>
        public TimeSpan GetTimeUntilSessionEnd(DateTime utcTime)
        {
            TimeSpan time = utcTime.TimeOfDay;

            if (CurrentSession == TradingSession.RTH)
            {
                // RTH ends at 21:45 UTC (1:45 PM PST)
                return time < RTH_END ? RTH_END - time : TimeSpan.Zero;
            }
            else // ETH
            {
                // ETH ends at 12:00 UTC (4:00 AM PST next day)
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
         * ═══════════════════════════════════════════════════════════════
         * SESSION TIMELINE EXAMPLES (PST/UTC)
         * ═══════════════════════════════════════════════════════════════
         * 
         * Example Day (Tuesday):
         * 
         * Monday 3:15 PM PST (Monday 23:15 UTC)    ─┬─ ETH starts
         * Monday 11:59 PM PST (Tuesday 07:59 UTC)  │  ETH continues
         * Tuesday 12:00 AM PST (Tuesday 08:00 UTC) │  ETH continues (overnight)
         * Tuesday 3:59 AM PST (Tuesday 11:59 UTC)  │  ETH continues
         * Tuesday 4:00 AM PST (Tuesday 12:00 UTC)  ─┴─ ETH ends, RTH starts ◄─┐
         * Tuesday 1:44 PM PST (Tuesday 21:44 UTC)  │  RTH continues           │
         * Tuesday 1:45 PM PST (Tuesday 21:45 UTC)  ─┘ RTH ends                │
         * Tuesday 1:46 PM PST (Tuesday 21:46 UTC)  ─┐ Gap period (no trading) │
         * Tuesday 3:14 PM PST (Tuesday 23:14 UTC)  ─┘ Gap period              │
         * Tuesday 3:15 PM PST (Tuesday 23:15 UTC)  ─┬─ ETH starts ◄───────────┘
         * Wednesday 3:59 AM PST (Wednesday 11:59 UTC) │ ETH continues
         * Wednesday 4:00 AM PST (Wednesday 12:00 UTC) ─┘ ETH ends, RTH starts
         * 
         * Session Durations:
         *   RTH: 9 hours 45 minutes (4:00 AM - 1:45 PM PST)
         *   ETH: 12 hours 45 minutes (3:15 PM - 4:00 AM PST next day)
         *   Gap: 1 hour 30 minutes (1:45 PM - 3:15 PM PST)
         * 
         * ═══════════════════════════════════════════════════════════════
         * API ALIGNMENT NOTES
         * ═══════════════════════════════════════════════════════════════
         * 
         * Quantower API Availability (Not Currently Used):
         * 
         * 1. Symbol.CurrentSessionsInfo (SessionsContainer)
         *    - Type: ISessionsContainer
         *    - Contains: Session times from data feed
         *    - Reason not used: CME RTH/ETH requires custom precision
         *    - Future: Could validate custom logic against feed sessions
         * 
         * 2. Symbol.Open / Symbol.PrevClose
         *    - Platform-managed session OHLC values
         *    - CipherFeed uses custom session open tracking in dictionaries
         *    - Reason: Need explicit control over session boundaries
         * 
         * 3. HistoryAggregation.SessionsContainer
         *    - Type: ISessionsContainer
         *    - Used in: Historical data requests for aggregations
         *    - Note: CipherFeed uses time-based session logic instead
         * 
         * Time Sources (Properly Used):
         * 
         * 1. Core.Instance.TimeUtils.DateTimeUtcNow
         *    ✓ Used in CipherFeed.cs::OnRun() for initialization
         *    ✓ Platform-synchronized UTC time
         * 
         * 2. Last.Time / Quote.Time
         *    ✓ Used in CheckBoundary() for tick-by-tick session detection
         *    ✓ Guaranteed UTC timestamps from market data
         * 
         * ═══════════════════════════════════════════════════════════════
         */
    }
}
