/*
 * FILE: CSVDataExporter.cs
 * PURPOSE: Real-time CSV export engine for market data (67 columns per tick)
 * KEY DEPENDENCIES: Core.TradingSession, Models.MarketDataSnapshot, Indicators.*
 * LAST MODIFIED: Updated to use proper namespace imports (no duplicated types)
 */

using CipherFeed.Core;
using CipherFeed.Indicators;
using CipherFeed.Models;
using System;
using System.Collections.Generic;
using System.IO;
using TradingPlatform.BusinessLayer;

namespace CipherFeed
{
    /// <summary>
    /// Handles CSV export of market data snapshots and indicator values
    /// Creates separate CSV file for each symbol to capture real-time tick data
    /// </summary>
    public class CSVDataExporter
    {
        private readonly string logDirectory;
        private readonly Dictionary<string, string> csvFilePaths = [];
        private readonly Dictionary<string, bool> headerWritten = [];
        private readonly Dictionary<string, object> fileLocks = [];
        private readonly TradingSession session;
        private readonly DateTime sessionStartTime;

        public CSVDataExporter(string logDirectory, TradingSession session, DateTime sessionStartTime)
        {
            this.logDirectory = logDirectory;
            this.session = session;
            this.sessionStartTime = sessionStartTime;

            // Create directory if it doesn't exist
            if (!Directory.Exists(logDirectory))
            {
                _ = Directory.CreateDirectory(logDirectory);
            }
        }

        /// <summary>
        /// Initialize CSV file for a specific symbol
        /// </summary>
        public void InitializeSymbolFile(string symbolRoot)
        {
            if (!csvFilePaths.ContainsKey(symbolRoot))
            {
                // Create filename with symbol, session, and timestamp
                string filename = $"{symbolRoot}_{session}_{sessionStartTime:yyyyMMdd_HHmmss}.csv";
                csvFilePaths[symbolRoot] = Path.Combine(logDirectory, filename);
                headerWritten[symbolRoot] = false;
                fileLocks[symbolRoot] = new object();
            }
        }

        /// <summary>
        /// Write a single snapshot to the appropriate symbol's CSV file
        /// </summary>
        public void WriteSnapshot(
            string symbolRoot,
            Symbol symbol,
            MarketDataSnapshot snapshot,
            double sessionOpen,
            SessionAnchoredVWAP vwapIndicator,
            VPOCIndicator vpocIndicator,
            TWAPIndicator twapIndicator)
        {
            // Initialize file if needed
            if (!csvFilePaths.ContainsKey(symbolRoot))
            {
                InitializeSymbolFile(symbolRoot);
            }

            lock (fileLocks[symbolRoot])
            {
                try
                {
                    using StreamWriter writer = new(csvFilePaths[symbolRoot], append: true);

                    // Write header if not written yet
                    if (!headerWritten[symbolRoot])
                    {
                        writer.WriteLine(GetCSVHeader());
                        headerWritten[symbolRoot] = true;
                    }

                    // Get indicator values
                    double vwap = double.NaN, vwapUpper = double.NaN, vwapLower = double.NaN;
                    double vwapUpperMPD = double.NaN, vwapLowerMPD = double.NaN;
                    double vpoc = double.NaN, vah = double.NaN, val = double.NaN;
                    double twap = double.NaN, twapUpper = double.NaN, twapLower = double.NaN;

                    if (vwapIndicator != null)
                    {
                        vwap = vwapIndicator.GetVWAP();
                        vwapUpper = vwapIndicator.GetUpperStdDev();
                        vwapLower = vwapIndicator.GetLowerStdDev();
                        vwapUpperMPD = vwapIndicator.GetUpperMPD();
                        vwapLowerMPD = vwapIndicator.GetLowerMPD();
                    }

                    if (vpocIndicator != null)
                    {
                        vpoc = vpocIndicator.GetVPOC();
                        vah = vpocIndicator.GetVAH();
                        val = vpocIndicator.GetVAL();
                    }

                    if (twapIndicator != null)
                    {
                        twap = twapIndicator.GetTWAP();
                        twapUpper = twapIndicator.GetUpperStdDev();
                        twapLower = twapIndicator.GetLowerStdDev();
                    }

                    // Build and write CSV row
                    string csvRow = BuildCSVRow(
                        snapshot.Timestamp,
                        symbolRoot,
                        snapshot,
                        sessionOpen,
                        vwap, vwapUpper, vwapLower, vwapUpperMPD, vwapLowerMPD,
                        vpoc, vah, val,
                        twap, twapUpper, twapLower);

                    writer.WriteLine(csvRow);
                }
                catch (Exception ex)
                {
                    // Log error but don't stop execution
                    Console.WriteLine($"Error writing to CSV for {symbolRoot}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Get CSV header with all 67 feature names
        /// </summary>
        private string GetCSVHeader()
        {
            List<string> headers = new()
            {
                // Metadata (2)
                "Timestamp",
                "SymbolName",

                // Orderflow Features (51)
                "Last",
                "Size",
                "Aggressor",
                "TickDirection",
                "BidPrice",
                "BidSize",
                "BidTickDirection",
                "AskPrice",
                "AskSize",
                "AskTickDirection",
                "Volume",
                "Trades",
                "Delta",
                "DeltaPercent",
                "CumulativeDelta",
                "BuyVolume",
                "BuyVolumePercent",
                "SellVolume",
                "SellVolumePercent",
                "BuyTrades",
                "SellTrades",
                "Imbalance",
                "ImbalancePercent",
                "AverageSize",
                "AverageBuySize",
                "AverageSellSize",
                "MaxOneTradeVolume",
                "MaxOneTradeVolumePercent",
                "FilteredVolume",
                "FilteredVolumePercent",
                "FilteredBuyVolume",
                "FilteredBuyVolumePercent",
                "FilteredSellVolume",
                "FilteredSellVolumePercent",
                "VWAPBid",
                "VWAPAsk",
                "CumulativeSizeBid",
                "CumulativeSizeAsk",
                "CumulativeSize",
                "BidsLiquidityChanges",
                "AsksLiquidityChanges",
                "BidsNumberOfChanges",
                "AsksNumberOfChanges",
                "LastTradeSize",
                "BidTradeSize",
                "AskTradeSize",
                "TimeBid",
                "TimeAsk",
                "SessionOpen",

                // Indicator Features (11)
                "VWAP",
                "VWAP_Upper_2StdDev",
                "VWAP_Lower_2StdDev",
                "VWAP_Upper_MPD",
                "VWAP_Lower_MPD",
                "VPOC",
                "VAH",
                "VAL",
                "TWAP",
                "TWAP_Upper_2StdDev",
                "TWAP_Lower_2StdDev",
                
                // Indicator Session Opens (3)
                "VWAP_SessionOpen",
                "VPOC_SessionOpen",
                "TWAP_SessionOpen"
            };

            return string.Join(",", headers);
        }

        /// <summary>
        /// Build a CSV row with all 67 features
        /// </summary>
        private string BuildCSVRow(
            DateTime timestamp,
            string symbolRoot,
            MarketDataSnapshot snapshot,
            double sessionOpen,
            double vwap, double vwapUpper, double vwapLower, double vwapUpperMPD, double vwapLowerMPD,
            double vpoc, double vah, double val,
            double twap, double twapUpper, double twapLower)
        {
            List<string> values = new()
            {
                // Metadata
                timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                symbolRoot,

                // Orderflow Features (51)
                snapshot.Last.ToString("F2"),
                snapshot.Size.ToString("F2"),
                ((int)snapshot.Aggressor).ToString(),
                ((int)snapshot.TickDirection).ToString(),
                snapshot.BidPrice.ToString("F2"),
                snapshot.BidSize.ToString("F2"),
                ((int)snapshot.BidTickDirection).ToString(),
                snapshot.AskPrice.ToString("F2"),
                snapshot.AskSize.ToString("F2"),
                ((int)snapshot.AskTickDirection).ToString(),
                snapshot.Volume.ToString("F2"),
                snapshot.Trades.ToString(),
                snapshot.Delta.ToString("F2"),
                snapshot.DeltaPercent.ToString("F4"),
                snapshot.CumulativeDelta.ToString("F2"),
                snapshot.BuyVolume.ToString("F2"),
                snapshot.BuyVolumePercent.ToString("F4"),
                snapshot.SellVolume.ToString("F2"),
                snapshot.SellVolumePercent.ToString("F4"),
                snapshot.BuyTrades.ToString(),
                snapshot.SellTrades.ToString(),
                snapshot.Imbalance.ToString("F2"),
                snapshot.ImbalancePercent.ToString("F4"),
                snapshot.AverageSize.ToString("F2"),
                snapshot.AverageBuySize.ToString("F2"),
                snapshot.AverageSellSize.ToString("F2"),
                snapshot.MaxOneTradeVolume.ToString("F2"),
                snapshot.MaxOneTradeVolumePercent.ToString("F4"),
                snapshot.FilteredVolume.ToString("F2"),
                snapshot.FilteredVolumePercent.ToString("F4"),
                snapshot.FilteredBuyVolume.ToString("F2"),
                snapshot.FilteredBuyVolumePercent.ToString("F4"),
                snapshot.FilteredSellVolume.ToString("F2"),
                snapshot.FilteredSellVolumePercent.ToString("F4"),
                snapshot.VWAPBid.ToString("F2"),
                snapshot.VWAPAsk.ToString("F2"),
                snapshot.CumulativeSizeBid.ToString("F2"),
                snapshot.CumulativeSizeAsk.ToString("F2"),
                snapshot.CumulativeSize.ToString("F2"),
                snapshot.BidsLiquidityChanges.ToString("F2"),
                snapshot.AsksLiquidityChanges.ToString("F2"),
                snapshot.BidsNumberOfChanges.ToString(),
                snapshot.AsksNumberOfChanges.ToString(),
                snapshot.LastTradeSize.ToString("F2"),
                snapshot.BidTradeSize.ToString("F2"),
                snapshot.AskTradeSize.ToString("F2"),
                snapshot.TimeBid.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                snapshot.TimeAsk.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                sessionOpen.ToString("F2"),

                // Indicator Features (11)
                FormatDouble(vwap),
                FormatDouble(vwapUpper),
                FormatDouble(vwapLower),
                FormatDouble(vwapUpperMPD),
                FormatDouble(vwapLowerMPD),
                FormatDouble(vpoc),
                FormatDouble(vah),
                FormatDouble(val),
                FormatDouble(twap),
                FormatDouble(twapUpper),
                FormatDouble(twapLower),
                
                // Indicator Session Opens (3)
                FormatDouble(snapshot.VWAPSessionOpen),
                FormatDouble(snapshot.VPOCSessionOpen),
                FormatDouble(snapshot.TWAPSessionOpen)
            };

            return string.Join(",", values);
        }

        /// <summary>
        /// Format double value for CSV (handle NaN)
        /// </summary>
        private string FormatDouble(double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value) ? "" : value.ToString("F6");
        }

        /// <summary>
        /// Get all CSV file paths
        /// </summary>
        public Dictionary<string, string> GetFilePaths()
        {
            return new Dictionary<string, string>(csvFilePaths);
        }

        /// <summary>
        /// Get CSV file path for a specific symbol
        /// </summary>
        public string GetFilePath(string symbolRoot)
        {
            return csvFilePaths.ContainsKey(symbolRoot) ? csvFilePaths[symbolRoot] : null;
        }
    }
}
