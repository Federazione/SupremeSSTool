using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SupremeScreenSharePro.Models;

namespace SupremeScreenSharePro.Modules
{
    public class ReportGenerator
    {
        public string GenerateHtmlReport(List<ScanResult> results)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Consolas', monospace; background-color: #1a1a1a; color: #ffffff; padding: 20px; }");
            sb.AppendLine("h1, h2 { border-bottom: 1px solid #ffffff; padding-bottom: 10px; }");
            sb.AppendLine(".module-section { margin-bottom: 30px; background-color: #2a2a2a; padding: 15px; border-radius: 5px; }");
            sb.AppendLine(".threat { color: #ff5555; font-weight: bold; }");
            sb.AppendLine(".clean { color: #55ff55; }");
            sb.AppendLine(".table-wrapper { overflow-x: auto; margin-top: 10px; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; table-layout: fixed; }");
            sb.AppendLine("th, td { border: 1px solid #444; padding: 8px; text-align: left; vertical-align: top; word-wrap: break-word; overflow-wrap: break-word; }");
            sb.AppendLine("th { background-color: #333; position: sticky; top: 0; z-index: 1; }");
            sb.AppendLine("td { max-width: 300px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<h1>Supreme ScreenShare Pro - Scan Report</h1>");
            sb.AppendLine($"<p>Scan Date: {DateTime.Now}</p>");

            foreach (var result in results)
            {
                sb.AppendLine("<div class='module-section'>");
                sb.AppendLine($"<h2>{result.ModuleName}</h2>");
                
                if (result.ThreatsFound > 0)
                {
                    sb.AppendLine($"<p class='threat'>Threats Found: {result.ThreatsFound}</p>");
                    
                    if (result.Details.Count > 0)
                    {
                        sb.AppendLine("<h3>Details:</h3>");
                        sb.AppendLine("<ul>");
                        foreach (var detail in result.Details)
                        {
                            sb.AppendLine($"<li>{detail}</li>");
                        }
                        sb.AppendLine("</ul>");
                    }

                    if (result.SuspiciousProcesses.Count > 0)
                    {
                        sb.AppendLine("<h3>Suspicious Processes:</h3>");
                        sb.AppendLine("<div class='table-wrapper'><table>");
                        sb.AppendLine("<tr><th>PID</th><th>Name</th><th>Path</th><th>Found Strings</th></tr>");
                        foreach (var proc in result.SuspiciousProcesses)
                        {
                            string stringsFound = proc.FoundStrings.Count > 0 ? string.Join(", ", proc.FoundStrings) : "None";
                            sb.AppendLine($"<tr><td>{proc.Id}</td><td>{proc.Name}</td><td>{proc.Path}</td><td>{stringsFound}</td></tr>");
                        }
                        sb.AppendLine("</table></div>");
                    }

                    if (result.SuspiciousFiles.Count > 0)
                    {
                        sb.AppendLine("<h3>Suspicious Files:</h3>");
                        sb.AppendLine("<div class='table-wrapper'><table>");
                        sb.AppendLine("<tr><th>File</th><th>Path</th><th>Size</th></tr>");
                        foreach (var file in result.SuspiciousFiles)
                        {
                            sb.AppendLine($"<tr><td>{file.FileName}</td><td>{file.FilePath}</td><td>{file.Size}</td></tr>");
                        }
                        sb.AppendLine("</table></div>");
                    }
                    
                    if (result.RegistryTraces.Count > 0)
                    {
                        sb.AppendLine("<h3>Registry Traces:</h3>");
                        sb.AppendLine("<div class='table-wrapper'><table>");
                        sb.AppendLine("<tr><th>Key</th><th>Value</th><th>Data</th></tr>");
                        foreach (var reg in result.RegistryTraces)
                        {
                            sb.AppendLine($"<tr><td>{reg.KeyPath}</td><td>{reg.ValueName}</td><td>{reg.ValueData}</td></tr>");
                        }
                        sb.AppendLine("</table></div>");
                    }
                }
                else
                {
                    sb.AppendLine("<p class='clean'>No threats found.</p>");
                }
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }
}
