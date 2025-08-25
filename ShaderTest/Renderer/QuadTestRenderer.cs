using Microsoft.Xna.Framework.Content;
using ShaderTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Renderer
{
    public class QuadTestRenderer : IRenderer
    {
        private VertexBuffer _fullScreenQuad;
        private VertexBuffer _triangle;
        private Effect _effect;
        private BasicEffect _basicEffect;

        public void Initialise(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _fullScreenQuad = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);

            _fullScreenQuad.SetData([
                new VertexPositionTexture(new Vector3(-1,  1,  0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3( 1,  1,  0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-1, -1,  0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3( 1, -1,  0), new Vector2(1, 1)),
            ]);

            static VertexPositionColor GeneratePointOnCircle(float radians, Color color)
            {
                return new VertexPositionColor(
                    new(MathF.Cos(radians), MathF.Sin(radians), 0f),
                    color
                );
            }

            _triangle = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 4, BufferUsage.WriteOnly);

            _triangle.SetData([
                GeneratePointOnCircle(MathF.Tau * 3f/12f, Color.Red),
                GeneratePointOnCircle(MathF.Tau * 11f/12f, Color.Blue),
                GeneratePointOnCircle(MathF.Tau * 7f/12f, Color.Green),
            ]);

            _effect = content.Load<Effect>("Shaders/QuadTest");
            _effect.CurrentTechnique = _effect.Techniques["DrawGradient"];

            _basicEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
            };
        }

        public void Render(GraphicsDevice graphicsDevice, RenderContext renderContext, SpriteBatch spriteBatch, List<ModelEntity> entities)
        {
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            graphicsDevice.SetVertexBuffer(_fullScreenQuad);

            _effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }
    }
}
