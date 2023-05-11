/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using RazorEngineCore;
using System.Text;

namespace Corsinvest.AppHero.Core.Emailing;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly ILogger<EmailTemplateService> _logger;

    public EmailTemplateService(ILogger<EmailTemplateService> logger) => _logger = logger;
    public string GenerateEmail<T>(string template, T mailTemplateModel) => new RazorEngine().Compile(template).Run(mailTemplateModel);
    public string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel) => GenerateEmail(GetTemplate(templateName), mailTemplateModel);

    public string GetTemplate(string fileName)
    {
        if (!File.Exists(fileName))
        {
            _logger.LogError("File not found! '{fileName}'", fileName);
            throw new FileNotFoundException("File not found", fileName);
        }

        using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        var mailText = sr.ReadToEnd();
        sr.Close();

        return mailText;
    }
}