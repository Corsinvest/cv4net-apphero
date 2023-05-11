/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Net;

namespace Corsinvest.AppHero.Core.MudBlazorUI.Shared.Components;

public partial class AHCustomError
{
    [EditorRequired][Parameter] public Exception Exception { get; set; } = default!;

    private string? Message { get; set; } = default!;
    private bool Showstacktrace { get; set; }
    private string Stacktrace { get; set; } = default!;
    private string StatusCode { get; set; } = HttpStatusCode.InternalServerError.ToString();

    protected override Task OnInitializedAsync()
    {
        switch (Exception)
        {
            //case CustomException e:
            //    StatusCode = e.StatusCode.ToString();
            //    if (e.ErrorMessages is not null) { Message = string.Join(", ", e.ErrorMessages.ToArray()); }
            //    break;

            default:
                if (Exception.InnerException != null)
                {
                    while (Exception.InnerException != null) { Exception = Exception.InnerException; }
                }
                Message = Exception.Message;
                break;
        }

        Logger.LogError(Exception, Message);

        Stacktrace = Exception.StackTrace!;
        return base.OnInitializedAsync();
    }
}