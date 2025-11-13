using Microsoft.Xna.Framework.Content;
using ShaderTest.Entities;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Renderer;

public class PbrForwardRenderer : IRenderer
{
    public void Initialise(ContentManager content, GraphicsDevice graphicsDevice)
    {
    }

    public void Render(GraphicsDevice graphicsDevice, RenderContext renderContext, SpriteBatch spriteBatch, List<ModelEntity> entities)
    {
        graphicsDevice.DepthStencilState = DepthStencilState.Default;
        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

        graphicsDevice.SetRenderTarget(null);
        graphicsDevice.Clear(Color.Black);

        foreach (var entity in entities)
        {
            entity.Draw(graphicsDevice, renderContext, GameShaders.Pbr);
        }
    }
}
