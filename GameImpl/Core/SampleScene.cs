using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Console;
using Nez.ImGuiTools;
using Nez.Tweens;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameImpl.Core {
    /// <summary>
    /// this entire class is one big sweet hack job to make adding samples easier. An exceptional hack is made so that we can render small
    /// pixel art scenes pixel perfect and still display our UI at a reasonable size.
    /// </summary>
    public abstract class SampleScene : Scene, IFinalRenderDelegate {
        public const int ScreenSpaceRenderLayer = 999;
        public UICanvas Canvas;

        ScreenSpaceRenderer _screenSpaceRenderer;
        static bool _needsFullRenderSizeForUi;


        public SampleScene(bool addExcludeRenderer = true, bool needsFullRenderSizeForUi = false) {
            _needsFullRenderSizeForUi = needsFullRenderSizeForUi;

            // setup one renderer in screen space for the UI and then (optionally) another renderer to render everything else
            if (needsFullRenderSizeForUi) {
                // dont actually add the renderer since we will manually call it later
                _screenSpaceRenderer = new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer);
                _screenSpaceRenderer.ShouldDebugRender = false;
                FinalRenderDelegate = this;
            } else {
                AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));
            }

            if (addExcludeRenderer)
                AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer));

            // create our canvas and put it on the screen space render layer
            Canvas = CreateEntity("ui").AddComponent(new UICanvas());
            Canvas.IsFullScreen = true;
            Canvas.RenderLayer = ScreenSpaceRenderLayer;
        }


        #region IFinalRenderDelegate

        private Scene _scene;

        public void OnAddedToScene(Scene scene) => _scene = scene;

        public void OnSceneBackBufferSizeChanged(int newWidth, int newHeight) => _screenSpaceRenderer.OnSceneBackBufferSizeChanged(newWidth, newHeight);

        public void HandleFinalRender(RenderTarget2D finalRenderTarget, Color letterboxColor, RenderTarget2D source,
                                      Rectangle finalRenderDestinationRect, SamplerState samplerState) {
            Nez.Core.GraphicsDevice.SetRenderTarget(null);
            Nez.Core.GraphicsDevice.Clear(letterboxColor);
            Graphics.Instance.Batcher.Begin(BlendState.Opaque, samplerState, DepthStencilState.None, RasterizerState.CullNone, null);
            Graphics.Instance.Batcher.Draw(source, finalRenderDestinationRect, Color.White);
            Graphics.Instance.Batcher.End();

            _screenSpaceRenderer.Render(_scene);
        }

        #endregion
    }

}
