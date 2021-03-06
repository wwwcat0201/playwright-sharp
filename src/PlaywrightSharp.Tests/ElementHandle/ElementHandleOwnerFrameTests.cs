﻿using System.Threading.Tasks;
using PlaywrightSharp.Tests.BaseTests;
using Xunit;
using Xunit.Abstractions;

namespace PlaywrightSharp.Tests.ElementHandle
{
    ///<playwright-file>elementhandle.spec.js</playwright-file>
    ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
    public class ElementHandleOwnerFrameTests : PlaywrightSharpPageBaseTest
    {
        internal ElementHandleOwnerFrameTests(ITestOutputHelper output) : base(output)
        {
        }

        ///<playwright-file>elementhandle.spec.js</playwright-file>
        ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
        ///<playwright-it>should work</playwright-it>
        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.Frames[1];
            var elementHandle = (IElementHandle)await frame.EvaluateHandleAsync("() => document.body");
            Assert.Equal(frame, await elementHandle.GetOwnerFrameAsync());
        }

        ///<playwright-file>elementhandle.spec.js</playwright-file>
        ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
        ///<playwright-it>should work for cross-process iframes</playwright-it>
        [Fact]
        public async Task ShouldWorkForCrossProcessIframes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.CrossProcessUrl + "/empty.html");
            var frame = Page.Frames[1];
            var elementHandle = (IElementHandle)await frame.EvaluateHandleAsync("() => document.body");
            Assert.Equal(frame, await elementHandle.GetOwnerFrameAsync());
        }

        ///<playwright-file>elementhandle.spec.js</playwright-file>
        ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
        ///<playwright-it>should work for document</playwright-it>
        [Fact]
        public async Task ShouldWorkForDocument()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.Frames[1];
            var elementHandle = (IElementHandle)await frame.EvaluateHandleAsync("() => document");
            Assert.Equal(frame, await elementHandle.GetOwnerFrameAsync());
        }

        ///<playwright-file>elementhandle.spec.js</playwright-file>
        ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
        ///<playwright-it>should work for iframe elements</playwright-it>
        [Fact]
        public async Task ShouldWorkForIframeElements()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            var elementHandle = (IElementHandle)await frame.EvaluateHandleAsync("() => document.querySelector('#frame1')");
            Assert.Equal(frame, await elementHandle.GetOwnerFrameAsync());
        }

        ///<playwright-file>elementhandle.spec.js</playwright-file>
        ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
        ///<playwright-it>should work for cross-frame evaluations</playwright-it>
        [Fact]
        public async Task ShouldWorkForCrossFrameEvaluations()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            var elementHandle = (IElementHandle)await frame.EvaluateHandleAsync("() => document.querySelector('#frame1').contentWindow.document.body");
            Assert.Equal(frame.ChildFrames[0], await elementHandle.GetOwnerFrameAsync());
        }

        ///<playwright-file>elementhandle.spec.js</playwright-file>
        ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
        ///<playwright-it>should work for detached elements</playwright-it>
        [Fact]
        public async Task ShouldWorkForDetachedElements()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var divHandle = (IElementHandle)await Page.EvaluateHandleAsync(@"() => {
                    var div = document.createElement('div');
                    document.body.appendChild(div);
                    return div;
                }");
            Assert.Equal(Page.MainFrame, await divHandle.GetOwnerFrameAsync());
            await Page.EvaluateAsync(@"() => {
                    var div = document.querySelector('div');
                    document.body.removeChild(div);
                }");
            Assert.Equal(Page.MainFrame, await divHandle.GetOwnerFrameAsync());
        }

        ///<playwright-file>elementhandle.spec.js</playwright-file>
        ///<playwright-describe>ElementHandle.ownerFrame</playwright-describe>
        ///<playwright-it>should work for adopted elements</playwright-it>
        [Fact(Skip = "Skipped in Playwright")]
        public async Task ShouldWorkForAdoptedElements()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var popupTask = Page.WaitForEvent<PopupEventArgs>(PageEvent.Popup);
            await Task.WhenAll(
              popupTask,
              Page.EvaluateAsync("url => window.__popup = window.open(url)", TestConstants.EmptyPage));
            var popup = await popupTask;
            var divHandle = (IElementHandle)await Page.EvaluateHandleAsync(@"() => {
                    var div = document.createElement('div');
                    document.body.appendChild(div);
                    return div;
                }");
            Assert.Equal(Page.MainFrame, await divHandle.GetOwnerFrameAsync());
            await Page.EvaluateAsync(@"() => {
                    var div = document.querySelector('div');
                    window.__popup.document.body.appendChild(div);
                }");
            Assert.Equal(Page.MainFrame, await divHandle.GetOwnerFrameAsync());
        }
    }
}
