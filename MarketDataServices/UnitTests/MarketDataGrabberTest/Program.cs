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
using Skywolf.MarketDataService.Restful;
using Newtonsoft.Json;
using Skywolf.Contracts.DataContracts.MarketData.TVC;
using Skywolf.Contracts.Services.Restful;

using System.Net.Http;
//{"data":"\n            \t<tr>\n        \t<td class=\"date bold center\">Oct 11, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Health-Sports Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Nov 01, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Austria\">&nbsp;<\/span><a href=\"\/markets\/austria\">Austria<\/a><\/td>\n            <td>Vienna Stock Exchange<\/td>\n            <td class=\"last\">All Saint's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hungary\">&nbsp;<\/span><a href=\"\/markets\/hungary\">Hungary<\/a><\/td>\n            <td>Budapest Stock Exchange<\/td>\n            <td class=\"last\">All Saint's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Poland\">&nbsp;<\/span><a href=\"\/markets\/poland\">Poland<\/a><\/td>\n            <td>Warsaw Stock Exchange<\/td>\n            <td class=\"last\">All Saint's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Nov 03, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Culture Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Nov 11, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Poland\">&nbsp;<\/span><a href=\"\/markets\/poland\">Poland<\/a><\/td>\n            <td>Warsaw Stock Exchange<\/td>\n            <td class=\"last\">National Holiday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Nov 22, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Independence Day Lebanon<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Nov 23, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Labour Thanksgiving Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Nov 25, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags USA\">&nbsp;<\/span><a href=\"\/markets\/united-states\">United States<\/a><\/td>\n            <td>New York Stock Exchange<\/td>\n            <td class=\"last\">Thanksgiving Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 08, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Austria\">&nbsp;<\/span><a href=\"\/markets\/austria\">Austria<\/a><\/td>\n            <td>Vienna Stock Exchange<\/td>\n            <td class=\"last\">Immaculate Conception<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 16, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Day of Reconciliation<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 23, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Emperor's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 24, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Austria\">&nbsp;<\/span><a href=\"\/markets\/austria\">Austria<\/a><\/td>\n            <td>Vienna Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Belgium\">&nbsp;<\/span><a href=\"\/markets\/belgium\">Belgium<\/a><\/td>\n            <td>Brussels Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Denmark\">&nbsp;<\/span><a href=\"\/markets\/denmark\">Denmark<\/a><\/td>\n            <td>Copenhagen Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Finland\">&nbsp;<\/span><a href=\"\/markets\/finland\">Finland<\/a><\/td>\n            <td>Helsinki Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags France\">&nbsp;<\/span><a href=\"\/markets\/france\">France<\/a><\/td>\n            <td>Paris Stock Exchange<\/td>\n            <td class=\"last\">Christmas - Early close at 14:00<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Germany\">&nbsp;<\/span><a href=\"\/markets\/germany\">Germany<\/a><\/td>\n            <td>Frankfurt Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hungary\">&nbsp;<\/span><a href=\"\/markets\/hungary\">Hungary<\/a><\/td>\n            <td>Budapest Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Italy\">&nbsp;<\/span><a href=\"\/markets\/italy\">Italy<\/a><\/td>\n            <td>Milan Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Netherlands\">&nbsp;<\/span><a href=\"\/markets\/netherlands\">Netherlands<\/a><\/td>\n            <td>Amsterdam Stock Exchange<\/td>\n            <td class=\"last\">Christmas - Early close at 14:00<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Norway\">&nbsp;<\/span><a href=\"\/markets\/norway\">Norway<\/a><\/td>\n            <td>Oslo Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Poland\">&nbsp;<\/span><a href=\"\/markets\/poland\">Poland<\/a><\/td>\n            <td>Warsaw Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Portugal\">&nbsp;<\/span><a href=\"\/markets\/portugal\">Portugal<\/a><\/td>\n            <td>Lisbon Stock Exchange<\/td>\n            <td class=\"last\">Christmas - Early close at 15:00<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Spain\">&nbsp;<\/span><a href=\"\/markets\/spain\">Spain<\/a><\/td>\n            <td>Madrid Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Sweden\">&nbsp;<\/span><a href=\"\/markets\/sweden\">Sweden<\/a><\/td>\n            <td>Stockholm Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Switzerland\">&nbsp;<\/span><a href=\"\/markets\/switzerland\">Switzerland<\/a><\/td>\n            <td>Switzerland Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags USA\">&nbsp;<\/span><a href=\"\/markets\/united-states\">United States<\/a><\/td>\n            <td>New York Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 25, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Jordan\">&nbsp;<\/span><a href=\"\/markets\/jordan\">Jordan<\/a><\/td>\n            <td>Amman Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Singapore\">&nbsp;<\/span><a href=\"\/markets\/singapore\">Singapore<\/a><\/td>\n            <td>Singapore Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Dubai\">&nbsp;<\/span><a href=\"\/markets\/dubai\">United Arab Emirates<\/a><\/td>\n            <td>Dubai Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 26, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Day of Goodwill<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 27, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Australia\">&nbsp;<\/span><a href=\"\/markets\/australia\">Australia<\/a><\/td>\n            <td>Sydney Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags UK\">&nbsp;<\/span><a href=\"\/markets\/united-kingdom\">United Kingdom<\/a><\/td>\n            <td>London Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 28, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Australia\">&nbsp;<\/span><a href=\"\/markets\/australia\">Australia<\/a><\/td>\n            <td>Sydney Stock Exchange<\/td>\n            <td class=\"last\">Boxing Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags UK\">&nbsp;<\/span><a href=\"\/markets\/united-kingdom\">United Kingdom<\/a><\/td>\n            <td>London Stock Exchange<\/td>\n            <td class=\"last\">Christmas<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 30, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Jordan\">&nbsp;<\/span><a href=\"\/markets\/jordan\">Jordan<\/a><\/td>\n            <td>Amman Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Dec 31, 2010<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Denmark\">&nbsp;<\/span><a href=\"\/markets\/denmark\">Denmark<\/a><\/td>\n            <td>Copenhagen Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Finland\">&nbsp;<\/span><a href=\"\/markets\/finland\">Finland<\/a><\/td>\n            <td>Helsinki Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Germany\">&nbsp;<\/span><a href=\"\/markets\/germany\">Germany<\/a><\/td>\n            <td>Frankfurt Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Italy\">&nbsp;<\/span><a href=\"\/markets\/italy\">Italy<\/a><\/td>\n            <td>Milan Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Netherlands\">&nbsp;<\/span><a href=\"\/markets\/netherlands\">Netherlands<\/a><\/td>\n            <td>Amsterdam Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Norway\">&nbsp;<\/span><a href=\"\/markets\/norway\">Norway<\/a><\/td>\n            <td>Oslo Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Portugal\">&nbsp;<\/span><a href=\"\/markets\/portugal\">Portugal<\/a><\/td>\n            <td>Lisbon Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Spain\">&nbsp;<\/span><a href=\"\/markets\/spain\">Spain<\/a><\/td>\n            <td>Madrid Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Sweden\">&nbsp;<\/span><a href=\"\/markets\/sweden\">Sweden<\/a><\/td>\n            <td>Stockholm Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Switzerland\">&nbsp;<\/span><a href=\"\/markets\/switzerland\">Switzerland<\/a><\/td>\n            <td>Switzerland Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 01, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Austria\">&nbsp;<\/span><a href=\"\/markets\/austria\">Austria<\/a><\/td>\n            <td>Vienna Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 03, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Australia\">&nbsp;<\/span><a href=\"\/markets\/australia\">Australia<\/a><\/td>\n            <td>Sydney Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Bank Holiday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags UK\">&nbsp;<\/span><a href=\"\/markets\/united-kingdom\">United Kingdom<\/a><\/td>\n            <td>London Stock Exchange<\/td>\n            <td class=\"last\">New Year's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 06, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Austria\">&nbsp;<\/span><a href=\"\/markets\/austria\">Austria<\/a><\/td>\n            <td>Vienna Stock Exchange<\/td>\n            <td class=\"last\">Epiphany Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Finland\">&nbsp;<\/span><a href=\"\/markets\/finland\">Finland<\/a><\/td>\n            <td>Helsinki Stock Exchange<\/td>\n            <td class=\"last\">Epiphany Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Armenian Christmas Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Sweden\">&nbsp;<\/span><a href=\"\/markets\/sweden\">Sweden<\/a><\/td>\n            <td>Stockholm Stock Exchange<\/td>\n            <td class=\"last\">Epiphany Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 10, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Coming of Age (Adults') Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 11, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Morocco\">&nbsp;<\/span><a href=\"\/markets\/morocco\">Morocco<\/a><\/td>\n            <td>Casablanca Stock Exchange<\/td>\n            <td class=\"last\">Proclamation of Independence<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 17, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags USA\">&nbsp;<\/span><a href=\"\/markets\/united-states\">United States<\/a><\/td>\n            <td>New York Stock Exchange<\/td>\n            <td class=\"last\">Martin Luther King, Jr. Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 25, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Police Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 26, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Australia\">&nbsp;<\/span><a href=\"\/markets\/australia\">Australia<\/a><\/td>\n            <td>Sydney Stock Exchange<\/td>\n            <td class=\"last\">Australia Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Jan 31, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 01, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 02, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 03, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Lunar New Year<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Singapore\">&nbsp;<\/span><a href=\"\/markets\/singapore\">Singapore<\/a><\/td>\n            <td>Singapore Stock Exchange<\/td>\n            <td class=\"last\">Lunar New Year<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 04, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Lunar New Year<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Singapore\">&nbsp;<\/span><a href=\"\/markets\/singapore\">Singapore<\/a><\/td>\n            <td>Singapore Stock Exchange<\/td>\n            <td class=\"last\">Lunar New Year<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 06, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 09, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">St. Maroun's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 10, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 11, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">National Founding Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 13, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 14, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 15, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Prophet's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Jordan\">&nbsp;<\/span><a href=\"\/markets\/jordan\">Jordan<\/a><\/td>\n            <td>Amman Stock Exchange<\/td>\n            <td class=\"last\">Prophet's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Prophet's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Dubai\">&nbsp;<\/span><a href=\"\/markets\/dubai\">United Arab Emirates<\/a><\/td>\n            <td>Dubai Stock Exchange<\/td>\n            <td class=\"last\">Prophet's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 16, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Morocco\">&nbsp;<\/span><a href=\"\/markets\/morocco\">Morocco<\/a><\/td>\n            <td>Casablanca Stock Exchange<\/td>\n            <td class=\"last\">Prophet's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 17, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Morocco\">&nbsp;<\/span><a href=\"\/markets\/morocco\">Morocco<\/a><\/td>\n            <td>Casablanca Stock Exchange<\/td>\n            <td class=\"last\">Prophet's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 20, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 21, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags USA\">&nbsp;<\/span><a href=\"\/markets\/united-states\">United States<\/a><\/td>\n            <td>New York Stock Exchange<\/td>\n            <td class=\"last\">Presidents' Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 22, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Italy\">&nbsp;<\/span><a href=\"\/markets\/italy\">Italy<\/a><\/td>\n            <td>Milan Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 23, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Russian_Federation\">&nbsp;<\/span><a href=\"\/markets\/russia\">Russia<\/a><\/td>\n            <td>Moscow Stock Exchange<\/td>\n            <td class=\"last\">Defender of the Fatherland Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 24, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 27, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Feb 28, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 01, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 02, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 03, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 07, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Argentina\">&nbsp;<\/span><a href=\"\/markets\/argentina\">Argentina<\/a><\/td>\n            <td>Buenos Aires Stock Exchange<\/td>\n            <td class=\"last\">Carnival<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Russian_Federation\">&nbsp;<\/span><a href=\"\/markets\/russia\">Russia<\/a><\/td>\n            <td>Moscow Stock Exchange<\/td>\n            <td class=\"last\">Women's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 08, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Argentina\">&nbsp;<\/span><a href=\"\/markets\/argentina\">Argentina<\/a><\/td>\n            <td>Buenos Aires Stock Exchange<\/td>\n            <td class=\"last\">Carnival<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Russian_Federation\">&nbsp;<\/span><a href=\"\/markets\/russia\">Russia<\/a><\/td>\n            <td>Moscow Stock Exchange<\/td>\n            <td class=\"last\">Women's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 09, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 10, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 13, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 14, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hungary\">&nbsp;<\/span><a href=\"\/markets\/hungary\">Hungary<\/a><\/td>\n            <td>Budapest Stock Exchange<\/td>\n            <td class=\"last\">Bridge Holiday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 15, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hungary\">&nbsp;<\/span><a href=\"\/markets\/hungary\">Hungary<\/a><\/td>\n            <td>Budapest Stock Exchange<\/td>\n            <td class=\"last\">1848 Revolution Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 16, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 17, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 20, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Israel\">&nbsp;<\/span><a href=\"\/markets\/israel\">Israel<\/a><\/td>\n            <td>Tel Aviv Stock Exchange<\/td>\n            <td class=\"last\">Purim<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 21, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Vernal Equinox<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Human Rights Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 22, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 23, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 24, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Argentina\">&nbsp;<\/span><a href=\"\/markets\/argentina\">Argentina<\/a><\/td>\n            <td>Buenos Aires Stock Exchange<\/td>\n            <td class=\"last\">Remembrance for Truth and Justice Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Markets Closed<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Mar 25, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Argentina\">&nbsp;<\/span><a href=\"\/markets\/argentina\">Argentina<\/a><\/td>\n            <td>Buenos Aires Stock Exchange<\/td>\n            <td class=\"last\">Bridge Holiday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Feast of Annunciation<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 05, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Ching Ming Festival<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 18, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Israel\">&nbsp;<\/span><a href=\"\/markets\/israel\">Israel<\/a><\/td>\n            <td>Tel Aviv Stock Exchange<\/td>\n            <td class=\"last\">Passover<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 19, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Israel\">&nbsp;<\/span><a href=\"\/markets\/israel\">Israel<\/a><\/td>\n            <td>Tel Aviv Stock Exchange<\/td>\n            <td class=\"last\">Passover<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 20, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Norway\">&nbsp;<\/span><a href=\"\/markets\/norway\">Norway<\/a><\/td>\n            <td>Oslo Stock Exchange<\/td>\n            <td class=\"last\">Easter - Early close at 13:00<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 21, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Argentina\">&nbsp;<\/span><a href=\"\/markets\/argentina\">Argentina<\/a><\/td>\n            <td>Buenos Aires Stock Exchange<\/td>\n            <td class=\"last\">Maundy Thursday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Denmark\">&nbsp;<\/span><a href=\"\/markets\/denmark\">Denmark<\/a><\/td>\n            <td>Copenhagen Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Norway\">&nbsp;<\/span><a href=\"\/markets\/norway\">Norway<\/a><\/td>\n            <td>Oslo Stock Exchange<\/td>\n            <td class=\"last\">Maundy Thursday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 22, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Argentina\">&nbsp;<\/span><a href=\"\/markets\/argentina\">Argentina<\/a><\/td>\n            <td>Buenos Aires Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Australia\">&nbsp;<\/span><a href=\"\/markets\/australia\">Australia<\/a><\/td>\n            <td>Sydney Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Austria\">&nbsp;<\/span><a href=\"\/markets\/austria\">Austria<\/a><\/td>\n            <td>Vienna Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Belgium\">&nbsp;<\/span><a href=\"\/markets\/belgium\">Belgium<\/a><\/td>\n            <td>Brussels Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Denmark\">&nbsp;<\/span><a href=\"\/markets\/denmark\">Denmark<\/a><\/td>\n            <td>Copenhagen Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Finland\">&nbsp;<\/span><a href=\"\/markets\/finland\">Finland<\/a><\/td>\n            <td>Helsinki Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags France\">&nbsp;<\/span><a href=\"\/markets\/france\">France<\/a><\/td>\n            <td>Paris Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Germany\">&nbsp;<\/span><a href=\"\/markets\/germany\">Germany<\/a><\/td>\n            <td>Frankfurt Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Italy\">&nbsp;<\/span><a href=\"\/markets\/italy\">Italy<\/a><\/td>\n            <td>Milan Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Netherlands\">&nbsp;<\/span><a href=\"\/markets\/netherlands\">Netherlands<\/a><\/td>\n            <td>Amsterdam Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Norway\">&nbsp;<\/span><a href=\"\/markets\/norway\">Norway<\/a><\/td>\n            <td>Oslo Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Portugal\">&nbsp;<\/span><a href=\"\/markets\/portugal\">Portugal<\/a><\/td>\n            <td>Lisbon Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Singapore\">&nbsp;<\/span><a href=\"\/markets\/singapore\">Singapore<\/a><\/td>\n            <td>Singapore Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Spain\">&nbsp;<\/span><a href=\"\/markets\/spain\">Spain<\/a><\/td>\n            <td>Madrid Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Sweden\">&nbsp;<\/span><a href=\"\/markets\/sweden\">Sweden<\/a><\/td>\n            <td>Stockholm Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Switzerland\">&nbsp;<\/span><a href=\"\/markets\/switzerland\">Switzerland<\/a><\/td>\n            <td>Switzerland Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags UK\">&nbsp;<\/span><a href=\"\/markets\/united-kingdom\">United Kingdom<\/a><\/td>\n            <td>London Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags USA\">&nbsp;<\/span><a href=\"\/markets\/united-states\">United States<\/a><\/td>\n            <td>New York Stock Exchange<\/td>\n            <td class=\"last\">Good Friday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 24, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Israel\">&nbsp;<\/span><a href=\"\/markets\/israel\">Israel<\/a><\/td>\n            <td>Tel Aviv Stock Exchange<\/td>\n            <td class=\"last\">Passover<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 25, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Australia\">&nbsp;<\/span><a href=\"\/markets\/australia\">Australia<\/a><\/td>\n            <td>Sydney Stock Exchange<\/td>\n            <td class=\"last\">ANZAC Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Austria\">&nbsp;<\/span><a href=\"\/markets\/austria\">Austria<\/a><\/td>\n            <td>Vienna Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Belgium\">&nbsp;<\/span><a href=\"\/markets\/belgium\">Belgium<\/a><\/td>\n            <td>Brussels Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Denmark\">&nbsp;<\/span><a href=\"\/markets\/denmark\">Denmark<\/a><\/td>\n            <td>Copenhagen Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Sinai Liberation Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Finland\">&nbsp;<\/span><a href=\"\/markets\/finland\">Finland<\/a><\/td>\n            <td>Helsinki Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags France\">&nbsp;<\/span><a href=\"\/markets\/france\">France<\/a><\/td>\n            <td>Paris Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Germany\">&nbsp;<\/span><a href=\"\/markets\/germany\">Germany<\/a><\/td>\n            <td>Frankfurt Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hungary\">&nbsp;<\/span><a href=\"\/markets\/hungary\">Hungary<\/a><\/td>\n            <td>Budapest Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Israel\">&nbsp;<\/span><a href=\"\/markets\/israel\">Israel<\/a><\/td>\n            <td>Tel Aviv Stock Exchange<\/td>\n            <td class=\"last\">Passover<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Italy\">&nbsp;<\/span><a href=\"\/markets\/italy\">Italy<\/a><\/td>\n            <td>Milan Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Netherlands\">&nbsp;<\/span><a href=\"\/markets\/netherlands\">Netherlands<\/a><\/td>\n            <td>Amsterdam Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Norway\">&nbsp;<\/span><a href=\"\/markets\/norway\">Norway<\/a><\/td>\n            <td>Oslo Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Poland\">&nbsp;<\/span><a href=\"\/markets\/poland\">Poland<\/a><\/td>\n            <td>Warsaw Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Portugal\">&nbsp;<\/span><a href=\"\/markets\/portugal\">Portugal<\/a><\/td>\n            <td>Lisbon Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Family Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Spain\">&nbsp;<\/span><a href=\"\/markets\/spain\">Spain<\/a><\/td>\n            <td>Madrid Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Sweden\">&nbsp;<\/span><a href=\"\/markets\/sweden\">Sweden<\/a><\/td>\n            <td>Stockholm Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Switzerland\">&nbsp;<\/span><a href=\"\/markets\/switzerland\">Switzerland<\/a><\/td>\n            <td>Switzerland Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags UK\">&nbsp;<\/span><a href=\"\/markets\/united-kingdom\">United Kingdom<\/a><\/td>\n            <td>London Stock Exchange<\/td>\n            <td class=\"last\">Easter<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 26, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Australia\">&nbsp;<\/span><a href=\"\/markets\/australia\">Australia<\/a><\/td>\n            <td>Sydney Stock Exchange<\/td>\n            <td class=\"last\">ANZAC Day Observance<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 27, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Freedom Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">Apr 29, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Showa Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags UK\">&nbsp;<\/span><a href=\"\/markets\/united-kingdom\">United Kingdom<\/a><\/td>\n            <td>London Stock Exchange<\/td>\n            <td class=\"last\">Bank Holiday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 01, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Egypt\">&nbsp;<\/span><a href=\"\/markets\/egypt\">Egypt<\/a><\/td>\n            <td>Egypt Stock Exchange<\/td>\n            <td class=\"last\">Labor Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Jordan\">&nbsp;<\/span><a href=\"\/markets\/jordan\">Jordan<\/a><\/td>\n            <td>Amman Stock Exchange<\/td>\n            <td class=\"last\">Labor Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Lebanon\">&nbsp;<\/span><a href=\"\/markets\/lebanon\">Lebanon<\/a><\/td>\n            <td>Beirut Stock Exchange<\/td>\n            <td class=\"last\">Labor Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Morocco\">&nbsp;<\/span><a href=\"\/markets\/morocco\">Morocco<\/a><\/td>\n            <td>Casablanca Stock Exchange<\/td>\n            <td class=\"last\">Labor Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Workers Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 02, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Labor Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Russian_Federation\">&nbsp;<\/span><a href=\"\/markets\/russia\">Russia<\/a><\/td>\n            <td>Moscow Stock Exchange<\/td>\n            <td class=\"last\">Workers Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Singapore\">&nbsp;<\/span><a href=\"\/markets\/singapore\">Singapore<\/a><\/td>\n            <td>Singapore Stock Exchange<\/td>\n            <td class=\"last\">Labor Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags South_Africa\">&nbsp;<\/span><a href=\"\/markets\/south-africa\">South Africa<\/a><\/td>\n            <td>Johannesburg Stock Exchange<\/td>\n            <td class=\"last\">Labor Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags UK\">&nbsp;<\/span><a href=\"\/markets\/united-kingdom\">United Kingdom<\/a><\/td>\n            <td>London Stock Exchange<\/td>\n            <td class=\"last\">May<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 03, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Constitution Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Poland\">&nbsp;<\/span><a href=\"\/markets\/poland\">Poland<\/a><\/td>\n            <td>Warsaw Stock Exchange<\/td>\n            <td class=\"last\">Constitution Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 04, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Greenery Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 05, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Japan\">&nbsp;<\/span><a href=\"\/markets\/japan\">Japan<\/a><\/td>\n            <td>Tokyo Stock Exchange<\/td>\n            <td class=\"last\">Children's Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 09, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Israel\">&nbsp;<\/span><a href=\"\/markets\/israel\">Israel<\/a><\/td>\n            <td>Tel Aviv Stock Exchange<\/td>\n            <td class=\"last\">Memorial Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Russian_Federation\">&nbsp;<\/span><a href=\"\/markets\/russia\">Russia<\/a><\/td>\n            <td>Moscow Stock Exchange<\/td>\n            <td class=\"last\">Victory Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 10, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Hong_Kong\">&nbsp;<\/span><a href=\"\/markets\/hong-kong\">Hong Kong<\/a><\/td>\n            <td>Hong Kong Stock Exchange<\/td>\n            <td class=\"last\">Buddha's Birthday<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\"><\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Israel\">&nbsp;<\/span><a href=\"\/markets\/israel\">Israel<\/a><\/td>\n            <td>Tel Aviv Stock Exchange<\/td>\n            <td class=\"last\">Independence Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 17, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Singapore\">&nbsp;<\/span><a href=\"\/markets\/singapore\">Singapore<\/a><\/td>\n            <td>Singapore Stock Exchange<\/td>\n            <td class=\"last\">Vesak Day<\/td>\n        <\/tr>\n              \t<tr>\n        \t<td class=\"date bold center\">May 20, 2011<\/td>\n            <td class=\"bold cur\"><span class=\"float_lang_base_1 ceFlags Denmark\">&nbsp;<\/span><a href=\"\/markets\/denmark\">Denmark<\/a><\/td>\n            <td>Copenhagen Stock Exchange<\/td>\n            <td class=\"last\">Holiday Denmark<\/td>\n        <\/tr>\n               \n ","noresult":0,"timeframe":"custom","dateFrom":"2000-08-02","dateTo":"2016-09-02","rows_num":200,"last_time_scope":1305849600,"bind_scroll_handler":true}
namespace MarketDataGrabberTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] symbols = new string[] { "ARNC", "AAPL", "ABMD", "ABT", "ADSK", "TAP", "ADBE", "ADI", "ADM", "AEP", "AES", "AFL", "HES", "AIG", "ALK", "AMAT", "AMD", "AMGN", "AON", "APA", "APD", "APH", "ADP", "AXP", "AZO", "BA", "BAC", "BAX", "BBBY", "BBY", "BDX", "BEN", "BK", "BLL", "BMY", "BSX", "CAG", "CAT", "C", "CAH", "CDNS", "CELG", "CERN", "CHD", "CI", "CL", "CLF", "CLX", "CMA", "CMCS_A", "CMS", "COG", "COST", "CPB", "CSCO", "CSX", "CTL", "CMI", "CY", "D", "DE", "DHR", "DIS", "DHI", "DTE", "DUK", "DVN", "ECL", "ED", "EFX", "EMR", "EOG", "EQT", "EA", "ETN", "ETR", "F", "FAST", "M", "FDX", "FISV", "FITB", "S", "NEE", "GD", "GE", "GILD", "GIS", "GLW", "GPS", "GWW", "HAL", "MNST", "HAS", "HBAN", "WELL", "HCP", "HD", "HFC", "HRL", "LHX", "HSY", "HUM", "HPQ", "IBM", "BIIB", "IFF", "INTC", "IP", "IPG", "IR", "ITW", "JBHT", "JNJ", "K", "KEY", "KLAC", "KMB", "KO", "KR", "KSS", "KSU", "LEN", "LLY", "LNC", "LOW", "LRCX", "LB", "LUV", "MAS", "MKC", "MCD", "MDT", "CVS", "MGM", "SPGI", "MMC", "MMM", "MO", "MHK", "MSI", "MRK", "MRO", "MSFT", "MTB", "MU", "MXIM", "MYL", "NBL", "NEM", "NKE", "JWN", "NOC", "NSC", "NTRS", "ES", "NUE", "NWL", "OKE", "OMC", "ORCL", "OXY", "PAYX", "PCAR", "PCG", "PEG", "PEP", "PFE", "PG", "PGR", "PH", "PHM", "PTC", "PNC", "PPG", "PPL", "PVH", "QCOM", "REGN", "ROK", "ROP", "ROST", "RTN", "T", "SBUX", "SCHW", "SHW", "SIVB", "SLB", "SNPS", "SO", "TRV", "STT", "STI", "SYK", "SWK", "SYMC", "SYY", "TER", "TIF", "TJX", "TMO", "TROW", "TSS", "TXN", "JCI", "TSN", "UNH", "UNP", "UTX", "VFC", "CBS", "VLO", "VMC", "VRTX", "WBA", "WDC", "WEC", "WFC", "WHR", "WMB", "WMT", "WSM", "WY", "X", "XLNX", "XOM", "FL", "ZION", "CHK", "AGN", "CB", "INTU", "MCHP", "ORLY", "RCL", "RIG", "ATVI", "HST", "INCY", "SPG", "MLM", "TSCO", "ALB", "AEO", "BRK_B", "SIRI", "O", "COF", "MCK", "DLTR", "LMT", "DRI", "LH", "DISH", "WAB", "FCX", "EL", "HSIC", "NTAP", "WAT", "CTXS", "HIG", "ALXN", "EIX", "OLED", "AABA", "CHKP", "ETFC", "DGX", "CIEN", "KMX", "AMTD", "IVZ", "TTWO", "AMZN", "BBT", "SRPT", "MS", "PXD", "CHRW", "NLY", "YUM", "FE", "URI", "VTR", "CTSH", "WM", "RRC", "CCI", "NVDA", "BKNG", "GS", "JNPR", "SBAC", "BLK", "UPS", "TGT", "EW", "MET", "ON", "MRVL", "ILMN", "VZ", "SJM", "XEL", "LNG", "TPR", "DVA", "EXC", "MCO", "ALGN", "EXAS", "GPN", "ADS", "MDLZ", "WLTW", "FIS", "ABC", "ZBH", "ANTM", "CVX", "AAP", "WW", "CNC", "PRU", "JBLU", "NFLX", "SWKS", "COP", "CNP", "DKS", "WYNN", "XEC", "CME", "EQIX", "STX", "CCL", "A", "AMT", "SRE", "PLD", "NOV", "EBAY", "RL", "ALL", "STZ", "PSA", "JPM", "USB", "HON", "ISRG", "MOH", "ACN", "WLL", "MAR", "NRG", "XPO", "CRM", "WCG", "DPZ", "IAC", "GOOG_L", "DLR", "LVS", "CE", "DXCM", "EXPE", "CF", "AMP", "ICE", "UAA", "VIA_B", "CMG", "UAL", "TDG", "MA", "HBI", "WU", "MLNX", "TMUS", "DAL", "CLR", "DFS", "TEL", "BX", "LULU", "CXO", "MELI", "VMW", "RF", "ULTA", "MSCI", "PM", "V", "AGNC", "DISC_A", "AVGO", "DG", "FTNT", "CHTR", "SSNC", "LYB", "TSLA", "KKR", "NXPI", "GM", "TRGP", "FRC", "FLT", "KMI", "HCA", "MOS", "MPC", "APTV", "TRIP", "WPX", "CPRI", "ZNGA", "CZR", "PSX", "SPLK", "FB", "NOW", "FIVE", "PANW", "WDAY", "FANG", "RH", "ABBV", "NCLH", "ZTS", "IQV", "COTY", "BURL", "VEEV", "TWTR", "TNDM", "AAL", "ARMK", "HLT", "ALLY", "GRUB", "PAYC", "ZEN", "PE", "ANET", "INFO", "SYF", "CFG", "CYBR", "W", "ZAYO", "KEYS", "ONCE", "GDDY", "ETSY", "TRU", "KHC", "PYPL", "HPE", "MTCH", "SQ", "TEAM", "FTV", "TWLO", "TTD", "NTNX", "COUP", "YUMC", "AA", "LW", "SNAP", "DXC", "AYX", "OKTA", "CVNA", "VST", "ATUS", "BHGE", "DD", "ROKU", "MDB", "VICI", "ZS", "DBX", "SPOT", "DOCU", "TLRY", "ELAN", "STNE", "LIN", "DELL", "FOXA", "DOW", "LYFT", "PINS", "ZM", "BYND", "UBER" };
                MarketDataRfService dataservice = new MarketDataRfService();
                string smb = "CMCSA,VIAB,NYSE:DATA,NYSE:CL,BRKB,NASDAQ:ON,NYSE:A,GOOGL,DISCA";
                string result = dataservice.GetStockBatchQuoteDict(smb, "tvc");
                Console.Write(result);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
           // try
           // {
           //     HttpClient client = new HttpClient();
                
           //     Task<HttpResponseMessage> response = client.GetAsync("http://tvc4.forexpros.com/");
           //     response.Wait();
           //     Task<string> responseBody = response.Result.Content.ReadAsStringAsync();
           //     responseBody.Wait();
           //     Console.WriteLine(responseBody.Result);
           // }
           // catch (Exception e)
           // {
           //     Console.WriteLine("\nException Caught!");
           //     Console.WriteLine("Message :{0} ", e.Message);
           // }
            //testDateTime();
            testTVCHttpGet();
            //testTVCGrabber();
            //testCalendarParser();
            //testCalendarInterface();

        }

        static void testCalendarInterface()
        {
            TVCMarketDataGrabber marketData = new TVCMarketDataGrabber();
            TVCCalendar[] calendars = marketData.GetCalendars(new DateTime(2000, 1, 1), new DateTime(2050, 9, 13));
            new MarketDataDatabase().TVC_StoreHolidays(calendars);
            Console.Write(calendars);
        }

        static void testCalendarParser()
        {
            TVCMarketDataGrabber marketData = new TVCMarketDataGrabber();
            TVCCalendarResponse response = marketData.RequestCalendar(new DateTime(2018, 1, 1), new DateTime(2018, 9, 1));
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response.data);
            HtmlNodeCollection trs = doc.DocumentNode.SelectNodes("tr");
            List<TVCCalendar> calenders = new List<TVCCalendar>();
            string lastdate = string.Empty;
            foreach (HtmlNode tr in trs)
            {
                HtmlNodeCollection tds = tr.SelectNodes("td");
                string date;
                string country;
                string exchange;
                string holiday;
                TVCCalendar calendar = new TVCCalendar();
                HtmlNodeCollection aNodes = tds[0].SelectNodes("a");
                if (aNodes != null && aNodes.Count > 0)
                {
                    date = aNodes[0].InnerText;
                }
                else
                {
                    date = tds[0].InnerText;
                }

                aNodes = tds[1].SelectNodes("a");
                if (aNodes != null && aNodes.Count > 0)
                {
                    country = aNodes[0].InnerText;
                }
                else
                {
                    country = tds[1].InnerText;
                }

                aNodes = tds[2].SelectNodes("a");
                if (aNodes != null && aNodes.Count > 0)
                {
                    exchange = aNodes[0].InnerText;
                }
                else
                {
                    exchange = tds[2].InnerText;
                }

                aNodes = tds[3].SelectNodes("a");
                if (aNodes != null && aNodes.Count > 0)
                {
                    holiday = aNodes[0].InnerText;
                }
                else
                {
                    holiday = tds[3].InnerText;
                }

                if (string.IsNullOrEmpty(date))
                {
                    date = lastdate;
                }
                else
                {
                    lastdate = date;
                }

                calendar.AsOfDate = Convert.ToDateTime(date);
                calendar.Country = country;
                calendar.Exchange = exchange;
                calendar.Holiday = holiday;
                calenders.Add(calendar);
            }

            Console.Write(calenders);
        }

        static void testCalendar()
        {
            BaseMarketDataGrabber marketData = new AVMarketDataGrabber();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Accept", "*/*");
            //headers.Add("Accept-Encoding", "gzip, deflate, br");
            headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            headers.Add("Host", "www.investing.com");
            headers.Add("Origin", "https://www.investing.com");
            headers.Add("Referer", "https://www.investing.com/holiday-calendar/");
            headers.Add("User-Agent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 68.0.3440.106 Safari / 537.36");
            headers.Add("X-Requested-With", "XMLHttpRequest");
            string result = marketData.HttpPost("https://www.investing.com/holiday-calendar/Service/getCalendarFilteredData", "dateFrom=2000-08-02&dateTo=2016-09-02&country=&currentTab=custom&limit_from=0", "application/x-www-form-urlencoded", headers);

            //string result = marketData.HttpPost("https://www.investing.com/holiday-calendar/Service/getCalendarFilteredData", "dateFrom=2000-08-02&dateTo=2016-09-02&country=&currentTab=custom&limit_from=1&showMore=true&submitFilters=0&last_time_scope=1305849600&byHandler=true", "application/x-www-form-urlencoded", headers);
            Console.Write(result);
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

            string[] symbols = new string[] { "ARNC", "AAPL", "ABMD", "ABT", "ADSK", "TAP", "ADBE", "ADI", "ADM", "AEP", "AES", "AFL", "HES", "AIG", "ALK", "AMAT", "AMD", "AMGN", "AON", "APA", "APD", "APH", "ADP", "AXP", "AZO", "BA", "BAC", "BAX", "BBBY", "BBY", "BDX", "BEN", "BK", "BLL", "BMY", "BSX", "CAG", "CAT", "C", "CAH", "CDNS", "CELG", "CERN", "CHD", "CI", "CL", "CLF", "CLX", "CMA", "CMCS_A", "CMS", "COG", "COST", "CPB", "CSCO", "CSX", "CTL", "CMI", "CY", "D", "DE", "DHR", "DIS", "DHI", "DTE", "DUK", "DVN", "ECL", "ED", "EFX", "EMR", "EOG", "EQT", "EA", "ETN", "ETR", "F", "FAST", "M", "FDX", "FISV", "FITB", "S", "NEE", "GD", "GE", "GILD", "GIS", "GLW", "GPS", "GWW", "HAL", "MNST", "HAS", "HBAN", "WELL", "HCP", "HD", "HFC", "HRL", "LHX", "HSY", "HUM", "HPQ", "IBM", "BIIB", "IFF", "INTC", "IP", "IPG", "IR", "ITW", "JBHT", "JNJ", "K", "KEY", "KLAC", "KMB", "KO", "KR", "KSS", "KSU", "LEN", "LLY", "LNC", "LOW", "LRCX", "LB", "LUV", "MAS", "MKC", "MCD", "MDT", "CVS", "MGM", "SPGI", "MMC", "MMM", "MO", "MHK", "MSI", "MRK", "MRO", "MSFT", "MTB", "MU", "MXIM", "MYL", "NBL", "NEM", "NKE", "JWN", "NOC", "NSC", "NTRS", "ES", "NUE", "NWL", "OKE", "OMC", "ORCL", "OXY", "PAYX", "PCAR", "PCG", "PEG", "PEP", "PFE", "PG", "PGR", "PH", "PHM", "PTC", "PNC", "PPG", "PPL", "PVH", "QCOM", "REGN", "ROK", "ROP", "ROST", "RTN", "T", "SBUX", "SCHW", "SHW", "SIVB", "SLB", "SNPS", "SO", "TRV", "STT", "STI", "SYK", "SWK", "SYMC", "SYY", "TER", "TIF", "TJX", "TMO", "TROW", "TSS", "TXN", "JCI", "TSN", "UNH", "UNP", "UTX", "VFC", "CBS", "VLO", "VMC", "VRTX", "WBA", "WDC", "WEC", "WFC", "WHR", "WMB", "WMT", "WSM", "WY", "X", "XLNX", "XOM", "FL", "ZION", "CHK", "AGN", "CB", "INTU", "MCHP", "ORLY", "RCL", "RIG", "ATVI", "HST", "INCY", "SPG", "MLM", "TSCO", "ALB", "AEO", "BRK_B", "SIRI", "O", "COF", "MCK", "DLTR", "LMT", "DRI", "LH", "DISH", "WAB", "FCX", "EL", "HSIC", "NTAP", "WAT", "CTXS", "HIG", "ALXN", "EIX", "OLED", "AABA", "CHKP", "ETFC", "DGX", "CIEN", "KMX", "AMTD", "IVZ", "TTWO", "AMZN", "BBT", "SRPT", "MS", "PXD", "CHRW", "NLY", "YUM", "FE", "URI", "VTR", "CTSH", "WM", "RRC", "CCI", "NVDA", "BKNG", "GS", "JNPR", "SBAC", "BLK", "UPS", "TGT", "EW", "MET", "ON", "MRVL", "ILMN", "VZ", "SJM", "XEL", "LNG", "TPR", "DVA", "EXC", "MCO", "ALGN", "EXAS", "GPN", "ADS", "MDLZ", "WLTW", "FIS", "ABC", "ZBH", "ANTM", "CVX", "AAP", "WW", "CNC", "PRU", "JBLU", "NFLX", "SWKS", "COP", "CNP", "DKS", "WYNN", "XEC", "CME", "EQIX", "STX", "CCL", "A", "AMT", "SRE", "PLD", "NOV", "EBAY", "RL", "ALL", "STZ", "PSA", "JPM", "USB", "HON", "ISRG", "MOH", "ACN", "WLL", "MAR", "NRG", "XPO", "CRM", "WCG", "DPZ", "IAC", "GOOG_L", "DLR", "LVS", "CE", "DXCM", "EXPE", "CF", "AMP", "ICE", "UAA", "VIA_B", "CMG", "UAL", "TDG", "MA", "HBI", "WU", "MLNX", "TMUS", "DAL", "CLR", "DFS", "TEL", "BX", "LULU", "CXO", "MELI", "VMW", "RF", "ULTA", "MSCI", "PM", "V", "AGNC", "DISC_A", "AVGO", "DG", "FTNT", "CHTR", "SSNC", "LYB", "TSLA", "KKR", "NXPI", "GM", "TRGP", "FRC", "FLT", "KMI", "HCA", "MOS", "MPC", "APTV", "TRIP", "WPX", "CPRI", "ZNGA", "CZR", "PSX", "SPLK", "FB", "NOW", "FIVE", "PANW", "WDAY", "FANG", "RH", "ABBV", "NCLH", "ZTS", "IQV", "COTY", "BURL", "VEEV", "TWTR", "TNDM", "AAL", "ARMK", "HLT", "ALLY", "GRUB", "PAYC", "ZEN", "PE", "ANET", "INFO", "SYF", "CFG", "CYBR", "W", "ZAYO", "KEYS", "ONCE", "GDDY", "ETSY", "TRU", "KHC", "PYPL", "HPE", "MTCH", "SQ", "TEAM", "FTV", "TWLO", "TTD", "NTNX", "COUP", "YUMC", "AA", "LW", "SNAP", "DXC", "AYX", "OKTA", "CVNA", "VST", "ATUS", "BHGE", "DD", "ROKU", "MDB", "VICI", "ZS", "DBX", "SPOT", "DOCU", "TLRY", "ELAN", "STNE", "LIN", "DELL", "FOXA", "DOW", "LYFT", "PINS", "ZM", "BYND", "UBER" };
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
