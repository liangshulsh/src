using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skywolf.MarketDataGrabber;
using Skywolf.DatabaseRepository;
using HtmlAgilityPack;
using ServiceStack.Text;
using Skywolf.Utility;
using System.Data;
using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.MarketDataService;
using Newtonsoft.Json;
using Skywolf.Contracts.DataContracts.MarketData.TVC;

namespace MarketDataGrabberTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //testDateTime();
            //testTVCHttpGet();
            testTVCGrabber();


        }
        static void testDateTime()
        {
            DateTime start = new DateTime(1970, 1, 1);
            DateTime currentDate = start.AddSeconds(728265600);
            double seconds = (currentDate - start).TotalSeconds;
            Console.Write(seconds);
            Console.Write(currentDate);
        }

        static void StoreTVCQuotes(IEnumerable<TVCQuoteResponse> quotes)
        {
            Task.Factory.StartNew(() =>
            {
                MarketDataDatabase marketData = new MarketDataDatabase();
                marketData.TVC_StoreQuotes(quotes);
            });
        }

        static void StoreTVCSymbols(IEnumerable<TVCSymbolResponse> symbols)
        {
            MarketDataDatabase marketData = new MarketDataDatabase();
            marketData.TVC_StoreSymbolList(symbols);
        }

        static Dictionary<string, TVCSymbolResponse> GetTVCSymbols(IEnumerable<string> symbols)
        {
            MarketDataDatabase marketData = new MarketDataDatabase();
            TVCSymbolResponse[] responses = marketData.TVC_GetSymbolList(symbols);
            if (responses != null)
            {
                return responses.ToDictionary(k => k.name, v => v);
            }
            else
            {
                return new Dictionary<string, TVCSymbolResponse>();
            }
        }

        static void testTVCGrabber()
        {
            TVCMarketDataGrabber marketData = new TVCMarketDataGrabber();
            marketData._getTVCSymbolsHandler = new Skywolf.MarketDataGrabber.GetTVCSymbols(GetTVCSymbols);
            marketData._updateTVCSymbolesHandler = new Skywolf.MarketDataGrabber.UpdateTVCSymbols(StoreTVCSymbols);
            marketData._updateTVCQuotesHandler = new Skywolf.MarketDataGrabber.UpdateTVCQuotes(StoreTVCQuotes);

            var searchresult = marketData.GetSymbolSearch("AA");

            Console.Write(searchresult);

            string[] symbols = new string[] { "A", "AABA", "AAL", "AAOI", "AAP", "AAPL", "ABBV", "ABC", "ABT", "ACN", "ADBE", "ADI", "ADM", "ADP", "ADS", "ADSK", "AEP", "AET", "AFL", "AGN", "AIG", "AKAM", "AKRX", "AKS", "ALB", "ALGN", "ALK", "ALL", "ALLY", "ALXN", "AMAT", "AMD", "AMGN", "AMP", "AMT", "AMTD", "AMZN", "ANDV", "ANET", "ANTM", "AON", "APA", "APC", "APD", "ARNC", "ATVI", "AVB", "AVGO", "AXP", "AYI", "AZO", "BA", "BAC", "BAX", "BBBY", "BBT", "BBY", "BCR", "BDX", "BEN", "BG", "BIIB", "BK", "BLK", "BLL", "BLUE", "BMRN", "BMY", "BRK_B", "BSX", "BURL", "BWA", "C", "CA", "CAG", "CAH", "CAR", "CAT", "CB", "CBI", "CBS", "CC", "CCI", "CCL", "CELG", "CERN", "CF", "CFG", "CHK", "CHKP", "CHRW", "CHTR", "CI", "CIEN", "CIT", "CL", "CLF", "CLR", "CLVS", "CLX", "CMA", "CMCS_A", "CME", "CMG", "CMI", "CMS", "CNC", "COF", "COG", "COH", "COL", "COMM", "COP", "COST", "COTY", "CPB", "CPN", "CRM", "CSCO", "CSX", "CTL", "CTSH", "CTXS", "CVS", "CVX", "CXO", "CY", "D", "DAL", "DE", "DFS", "DG", "DGX", "DHI", "DHR", "DIS", "DISC_A", "DISH", "DKS", "DLPH", "DLR", "DLTR", "DOV", "DPS", "DPZ", "DRI", "DUK", "DVA", "DVN", "DXC", "DXCM", "EA", "EBAY", "ECL", "ED", "EFX", "EIX", "EL", "EMN", "EMR", "ENDP", "EOG", "EQIX", "EQR", "EQT", "ESRX", "ESV", "ETFC", "ETN", "EVHC", "EW", "EXAS", "EXC", "EXEL", "EXPE", "F", "FANG", "FAST", "FB", "FCX", "FDX", "FE", "FEYE", "FFIV", "FIS", "FISV", "FITB", "FL", "FLT", "FMC", "FNSR", "FOXA", "FRC", "FSLR", "GD", "GE", "GGP", "GILD", "GIS", "GLW", "GM", "GOOG_L", "GPN", "GPS", "GRUB", "GS", "GT", "GWW", "HAL", "HAS", "HBAN", "HBI", "HCA", "HCN", "HCP", "HD", "HDS", "HES", "HFC", "HIG", "HLT", "HOG", "HOLX", "HON", "HP", "HPE", "HPQ", "HST", "HSY", "HTZ", "HUM", "HUN", "IBM", "ICE", "IDXX", "ILMN", "INCY", "INFO", "INTC", "INTU", "IP", "IPG", "IR", "ISRG", "ITW", "IVZ", "JBHT", "JBLU", "JCI", "JCP", "JNJ", "JNPR", "JPM", "JWN", "K", "KEY", "KHC", "KIM", "KITE", "KLAC", "KMB", "KMI", "KMX", "KO", "KORS", "KR", "KSS", "KSU", "LB", "LBTY_A", "LEA", "LEN", "LH", "LITE", "LLY", "LMT", "LNC", "LNG", "LOW", "LRCX", "LULU", "LUV", "LVLT", "LVS", "LYB", "M", "MA", "MAR", "MAS", "MAT", "MCD", "MCHP", "MCK", "MCO", "MDLZ", "MDT", "MELI", "MET", "MGM", "MHK", "MLM", "MMC", "MMM", "MNK", "MNST", "MO", "MON", "MOS", "MPC", "MRK", "MRO", "MRVL", "MS", "MSFT", "MSI", "MTB", "MU", "MXIM", "MYL", "NBL", "NBR", "NCLH", "NEE", "NEM", "NFLX", "NFX", "NKE", "NLSN", "NLY", "NOC", "NOV", "NOW", "NRG", "NSC", "NTAP", "NTRS", "NUE", "NVDA", "NWL", "NXPI", "O", "OAS", "OCLR", "OKE", "OMC", "ON", "ORCL", "ORLY", "OXY", "P", "PANW", "PAYX", "PCAR", "PCG", "PCLN", "PE", "PEG", "PEP", "PFE", "PG", "PGR", "PH", "PHM", "PLD", "PM", "PNC", "PPG", "PPL", "PRGO", "PRU", "PSA", "PSX", "PTEN", "PVH", "PX", "PXD", "PYPL", "Q", "QCOM", "QRVO", "RAD", "RCL", "REGN", "RF", "RH", "RHT", "RICE", "RIG", "RL", "ROK", "ROST", "RRC", "RTN", "S", "SBAC", "SBUX", "SCHW", "SEE", "SHOP", "SHW", "SIG", "SINA", "SIRI", "SJM", "SLB", "SLCA", "SM", "SNA", "SNI", "SO", "SPG", "SPGI", "SPLK", "SRC", "SRE", "SRPT", "STI", "STLD", "STT", "STX", "STZ", "SWK", "SWKS", "SWN", "SYF", "SYK", "SYMC", "SYY", "T", "TAP", "TDG", "TEL", "TER", "TGT", "TIF", "TJX", "TMO", "TMUS", "TRGP", "TRIP", "TROW", "TRV", "TSCO", "TSLA", "TSN", "TSRO", "TTWO", "TWTR", "TWX", "TXN", "UAA", "UAL", "UHS", "ULTA", "UNH", "UNP", "UPS", "URI", "USB", "UTX", "V", "VFC", "VLO", "VMC", "VMW", "VNTV", "VRTX", "VTR", "VZ", "W", "WBA", "WDAY", "WDC", "WEC", "WFC", "WFT", "WHR", "WLL", "WLTW", "WM", "WMB", "WMT", "WPX", "WSM", "WU", "WY", "WYN", "WYNN", "X", "XEC", "XEL", "XLNX", "XOM", "XPO", "XRAY", "YELP", "YUM", "ZBH", "ZION", "ZTS" };
            //string[] symbols = new string[] { "A", "AABA", "AAL", "AAOI", "AAP", "AAPL", "ABBV", "ABC" };

            Quote[] quotes_output = marketData.StockBatchQuote(symbols);

            Console.Write(quotes_output);

            TimeSeriesDataOutput output = marketData.GetTimeSeriesData(new TimeSeriesDataInput()
            {
                Frequency = BarFrequency.Day1,
                IsAdjustedValue = false,
                OutputCount = 10,
                Symbol = "MSFT"
            });

            Console.Write(output);

            TVCHistoryResponse history = marketData.GetHistoricalPrices("AAPL", BarFrequency.Month1, new DateTime(1980, 1, 1), new DateTime(2018, 9, 6));
            TVCQuotesResponse quotes = marketData.GetQuotes(new string[] { "shanghai:601988", "shanghai: 603088" });
            TVCSymbolResponse symbol = marketData.GetSymbolInfo("AAPL");
            Console.Write(history);
            Console.Write(quotes);
            Console.Write(symbol);

            Console.Read();
        }

        static void testTVCHttpGet()
        {
            BaseMarketDataGrabber marketData = new TVCMarketDataGrabber();
            string result = marketData.HttpGet("https://tvc4.forexpros.com/");
            string keyword_carrier = "carrier=";
            string keyword_time = "time=";
            
            int carrierIdx = result.IndexOf(keyword_carrier);
            int andIdx = result.IndexOf("&", carrierIdx);
            string carrier = result.Substring(carrierIdx + keyword_carrier.Length, andIdx-(carrierIdx+keyword_carrier.Length));

            int timeIdx = result.IndexOf(keyword_time, carrierIdx);
            andIdx = result.IndexOf("&", timeIdx);
            string time = result.Substring(timeIdx + keyword_time.Length, andIdx-(timeIdx+keyword_time.Length));

            string prices = marketData.HttpGet(string.Format("https://tvc4.forexpros.com/{0}/{1}/1/1/8/history?symbol=AAPL&resolution=M&from=603015941&to=1536136001", carrier, time));
            string quote = marketData.HttpGet(string.Format("https://tvc4.forexpros.com/{0}/{1}/1/1/8/quotes?symbols=shanghai:601988,shanghai:603088", carrier, time));
            string symbol = marketData.HttpGet(string.Format("https://tvc4.forexpros.com/{0}/{1}/1/1/8/symbols?symbol=aapl", carrier, time));

            TVCHistoryResponse priceObj = JsonConvert.DeserializeObject<TVCHistoryResponse>(prices);
            TVCQuotesResponse quotesObj = JsonConvert.DeserializeObject<TVCQuotesResponse>(quote);
            TVCSymbolResponse symbolObj = JsonConvert.DeserializeObject<TVCSymbolResponse>(symbol);
            // resolution=M|W|D|60|10|5|1
            Console.Write(carrier);
            Console.Write(time);
            Console.Write(priceObj);
            Console.Write(quotesObj);
            Console.Write(symbolObj);
            Console.Write(result);
        }

        static void testHttpGet()
        {
            BaseMarketDataGrabber marketData = new AVMarketDataGrabber();
            string result = marketData.HttpGet("https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_WEEKLY&symbol=BTC&market=CNY&apikey=apikey=GCN17TO8N1K4JU9G&datatype=csv");
            DataTable dt = TextUtility.ConvertCSVToTable(result, "prices");

            Console.Write(dt);
        }

        static void testHtmlGet()
        {
            BaseMarketDataGrabber marketData = new AVMarketDataGrabber();
            HtmlDocument doc = marketData.HtmlGet("https://www.investing.com/");
            string result = doc.ToString();
            Console.Write(result);
        }

        #region Stock List
        static string[] _stockList = new string[] {
                "AAPL",
"MSFT",
"AMZN",
"BRKB",
"FB",
"JPM",
"JNJ",
"XOM",
"GOOG",
"GOOGL",
"BAC",
"INTC",
"WFC",
"T",
"CVX",
"V",
"PFE",
"HD",
"UNH",
"CSCO",
"PG",
"VZ",
"BA",
"C",
"KO",
"MA",
"CMCSA",
"PEP",
"PM",
"DIS",
"ABBV",
"DWDP",
"MRK",
"NVDA",
"ORCL",
"IBM",
"MMM",
"WMT",
"NFLX",
"MCD",
"MO",
"GE",
"AMGN",
"MDT",
"HON",
"ADBE",
"UNP",
"ABT",
"BMY",
"TXN",
"BKNG",
"GILD",
"AVGO",
"ACN",
"UTX",
"SLB",
"GS",
"CAT",
"NKE",
"PYPL",
"LMT",
"TMO",
"COST",
"QCOM",
"SBUX",
"CRM",
"USB",
"NEE",
"LLY",
"MS",
"TWX",
"LOW",
"UPS",
"PNC",
"COP",
"AXP",
"CELG",
"BLK",
"AMT",
"CB",
"CVS",
"CL",
"SCHW",
"RTN",
"MDLZ",
"GD",
"EOG",
"NOC",
"MU",
"DHR",
"FDX",
"AMAT",
"BIIB",
"CHTR",
"BDX",
"ANTM",
"WBA",
"AGN",
"AET",
"CME",
"DUK",
"BK",
"SYK",
"MON",
"TJX",
"ATVI",
"DE",
"ADP",
"OXY",
"CSX",
"AIG",
"SPGI",
"ITW",
"SPG",
"MET",
"CTSH",
"COF",
"ISRG",
"GM",
"CCI",
"SO",
"D",
"PRU",
"EMR",
"F",
"ICE",
"INTU",
"MMC",
"PX",
"VRTX",
"MAR",
"HAL",
"CI",
"ZTS",
"BBT",
"VLO",
"PSX",
"STZ",
"ESRX",
"KMB",
"NSC",
"FOXA",
"EBAY",
"EXC",
"TRV",
"TGT",
"BSX",
"EA",
"KHC",
"HUM",
"STT",
"HPQ",
"DAL",
"ECL",
"PGR",
"ETN",
"TEL",
"APD",
"ILMN",
"MPC",
"AON",
"LYB",
"AFL",
"ADI",
"ALL",
"EL",
"AEP",
"PLD",
"WM",
"EQIX",
"LRCX",
"APC",
"JCI",
"SHW",
"BAX",
"FIS",
"STI",
"LUV",
"PSA",
"USD",
"ROST",
"FISV",
"EW",
"PXD",
"MCK",
"ROP",
"SYY",
"DXC",
"KMI",
"SRE",
"PPG",
"YUM",
"ADSK",
"MTB",
"WDC",
"HPE",
"HCA",
"MCO",
"CCL",
"REGN",
"RHT",
"WY",
"TROW",
"APH",
"GIS",
"DFS",
"PEG",
"CMI",
"ALXN",
"VFC",
"ADM",
"GLW",
"ED",
"DG",
"SYF",
"FTV",
"MNST",
"FCX",
"SWK",
"OKE",
"PCAR",
"XEL",
"AVB",
"PH",
"APTV",
"PCG",
"EQR",
"DLTR",
"CXO",
"COL",
"ROK",
"IP",
"ZBH",
"FITB",
"AAL",
"NTRS",
"TSN",
"DLR",
"AMP",
"A",
"MCHP",
"IR",
"DPS",
"MYL",
"KR",
"NEM",
"RF",
"EIX",
"KEY",
"ORLY",
"WMB",
"CFG",
"WELL",
"WLTW",
"RCL",
"SBAC",
"CAH",
"WEC",
"PAYX",
"PPL",
"NUE",
"BXP",
"HRS",
"DTE",
"ES",
"CNC",
"XLNX",
"CERN",
"HIG",
"SWKS",
"ALGN",
"MGM",
"GPN",
"BBY",
"CBS",
"AZO",
"VTR",
"AME",
"INFO",
"NKTR",
"CLX",
"MSI",
"KLAC",
"DVN",
"UAL",
"IDXX",
"OMC",
"HBAN",
"LH",
"STX",
"CMA",
"PFG",
"NTAP",
"WRK",
"LEN",
"LLL",
"VRSK",
"K",
"CTL",
"SYMC",
"HLT",
"FOX",
"LNC",
"ESS",
"FAST",
"WAT",
"TXT",
"VMC",
"FE",
"DHI",
"EMN",
"DOV",
"NBL",
"TPR",
"TDG",
"O",
"RSG",
"APA",
"CAG",
"ETFC",
"CTAS",
"MHK",
"AWK",
"INCY",
"WYNN",
"MTD",
"URI",
"GWW",
"IQV",
"ANDV",
"CBRE",
"BFB",
"ETR",
"TSS",
"XL",
"RMD",
"NOV",
"EFX",
"SJM",
"TAP",
"ABC",
"XYL",
"HSY",
"BLL",
"AEE",
"MRO",
"HST",
"HES",
"DGX",
"EXPE",
"L",
"CHRW",
"IVZ",
"GPC",
"ANSS",
"GGP",
"MLM",
"MKC",
"FTI",
"CBOE",
"SIVB",
"CMS",
"MAS",
"ARE",
"AJG",
"NWL",
"SNPS",
"CHD",
"AKAM",
"CTXS",
"ULTA",
"VNO",
"LKQ",
"BHGE",
"EQT",
"CNP",
"HII",
"RJF",
"PVH",
"XRAY",
"WYN",
"KSU",
"TTWO",
"COO",
"PNR",
"BEN",
"EXR",
"VAR",
"EXPD",
"KMX",
"PRGO",
"KSS",
"CINF",
"COG",
"HCP",
"NCLH",
"WHR",
"VIAB",
"PKG",
"IFF",
"IT",
"DRI",
"CA",
"CDNS",
"NLSN",
"BLKFDS",
"RE",
"HOLX",
"MAA",
"UNM",
"HSIC",
"UHS",
"ADS",
"ZION",
"AMG",
"ALB",
"JBHT",
"FMC",
"NDAQ",
"BWA",
"TIF",
"ARNC",
"VRSN",
"DRE",
"DVA",
"HRL",
"LNT",
"HAS",
"LB",
"UDR",
"KORS",
"AVY",
"IRM",
"NRG",
"AOS",
"WU",
"CF",
"IPGP",
"M",
"FBHS",
"XEC",
"IPG",
"QRVO",
"TMK",
"FFIV",
"PNW",
"SLG",
"REG",
"DISH",
"AAP",
"COTY",
"FRT",
"PKI",
"MOS",
"JNPR",
"AMD",
"CPB",
"SNA",
"ALLE",
"NI",
"CMG",
"FLR",
"ALK",
"TSCO",
"PHM",
"AES",
"LUK",
"RHI",
"SEE",
"JEC",
"HP",
"HOG",
"FLIR",
"GPS",
"CSRA",
"HBI",
"PBCT",
"AIV",
"GRMN",
"XRX",
"GT",
"NWSA",
"MAC",
"KIM",
"DISCK",
"RL",
"LEG",
"AYI",
"JWN",
"FLS",
"FL",
"SCG",
"HRB",
"SRCL",
"PWR",
"BHF",
"NFX",
"AIZ",
"EVHC",
"TRIP",
"MAT",
"NAVI",
"RRC",
"DISCA",
"UAA",
"UA",
"NWS",
"UBFUT",
"ESM8"};

        #endregion

        static void testAVBatchQuote()
        {
            AVMarketDataGrabber av = new AVMarketDataGrabber();
            MarketDataDatabase marketData = new MarketDataDatabase();
            av.UpdateAPIKeys(marketData.VA_GetAvailableAPIKey(1));
            Quote[] quotes = av.StockBatchQuote(_stockList);
            Console.Write(quotes);
            
        }

        static void testAVStore()
        {
            try
            {
                MarketDataService service = new MarketDataService();
                TimeSeriesDataInput input = new TimeSeriesDataInput();
                input.Frequency = BarFrequency.Minute1;
                input.IsAdjustedValue = false;
                input.Symbol = "DNO";
                input.OutputCount = -1;
                TimeSeriesDataOutput output = service.GetTimeSeriesData(input, "av");
                service.VA_StorePrices(200000003, BarFrequency.Minute1, false, output.Data);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }
    }
}
