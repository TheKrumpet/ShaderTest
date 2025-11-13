using Microsoft.Xna.Framework.Content;
using ShaderTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Renderer;

public interface IRenderer
{
    public void Initialise(ContentManager content, GraphicsDevice graphicsDevice);
    public void Render(GraphicsDevice graphicsDevice, RenderContext renderContext, SpriteBatch spriteBatch, List<ModelEntity> entities);
}
