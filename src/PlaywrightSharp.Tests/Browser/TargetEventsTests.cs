﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlaywrightSharp.Tests.BaseTests;
using Xunit;
using Xunit.Abstractions;

namespace PlaywrightSharp.Tests.Browser
{
    ///<playwright-file>launcher.spec.js</playwright-file>
    ///<playwright-describe>Browser target events</playwright-describe>
    public class TargetEventsTests : PlaywrightSharpBrowserContextBaseTest
    {
        /// <inheritdoc/>
        public TargetEventsTests(ITestOutputHelper output) : base(output)
        {
        }

        ///<playwright-file>launcher.spec.js</playwright-file>
        ///<playwright-describe>Browser target events</playwright-describe>
        ///<playwright-it>should work</playwright-it>
        [Fact]
        public async Task ShouldWork()
        {
            var browser = await Playwright.LaunchAsync(TestConstants.DefaultBrowserOptions);
            var events = new List<string>();
            browser.TargetCreated += (sender, e) => events.Add("CREATED: " + e.Target.Url);
            browser.TargetChanged += (sender, e) => events.Add("CHANGED: " + e.Target.Url);
            browser.TargetDestroyed += (sender, e) => events.Add("DESTROYED: " + e.Target.Url);
            var page = await browser.DefaultContext.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();
            Assert.Equal(new[] {
                $"CREATED: {TestConstants.AboutBlank}",
                $"CHANGED: {TestConstants.EmptyPage}",
                $"DESTROYED: {TestConstants.EmptyPage}"
            }, events);
            await browser.CloseAsync();
        }
    }
}
