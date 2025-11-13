using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderTest.Entities;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShaderTest.GameDebug;

namespace ShaderTest.Renderer;

public class PbrDeferredRenderer : IRenderer
{
    public RenderTarget2D AlbedoMap;
    public RenderTarget2D NormalMap;
    public RenderTarget2D PbrMap;
    public RenderTarget2D DepthMap;
    private RenderTargetBinding[] _deferredRenderTargetBindings;
    private VertexBuffer _fullScreenQuad;
    private Effect _debugEffect;

    public void Initialise(ContentManager content, GraphicsDevice graphicsDevice)
    {
        _fullScreenQuad = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);

        _fullScreenQuad.SetData([
            new VertexPositionTexture(new Vector3(-1,  1,  0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3( 1,  1,  0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-1, -1,  0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3( 1, -1,  0), new Vector2(1, 1)),
        ]);

        var vs = graphicsDevice.Viewport.Bounds;

        AlbedoMap = new RenderTarget2D(graphicsDevice, vs.Width, vs.Height, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents)
        {
            Name = "Albedo"
        };

        NormalMap = new RenderTarget2D(graphicsDevice, vs.Width, vs.Height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents)
        {
            Name = "Normal"
        };

        PbrMap = new RenderTarget2D(graphicsDevice, vs.Width, vs.Height, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents)
        {
            Name = "PBR"
        };

        DepthMap = new RenderTarget2D(graphicsDevice, vs.Width, vs.Height, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents)
        {
            Name = "Depth"
        };

        _debugEffect = content.Load<Effect>("Shaders/QuadTest");
        _debugEffect.CurrentTechnique = _debugEffect.Techniques["DrawGradient"];

        _deferredRenderTargetBindings =
        [
            new RenderTargetBinding(AlbedoMap),
            new RenderTargetBinding(NormalMap),
            new RenderTargetBinding(DepthMap),
            new RenderTargetBinding(PbrMap),
        ];

        GameShaders.PbrDeferred.AlbedoMap = AlbedoMap;
        GameShaders.PbrDeferred.NormalMap = NormalMap;
        GameShaders.PbrDeferred.PBRMap = PbrMap;
        GameShaders.PbrDeferred.DepthMap = DepthMap;
    }

    public void Render(GraphicsDevice graphicsDevice, RenderContext renderContext, SpriteBatch spriteBatch, List<ModelEntity> entities)
    {
        graphicsDevice.BlendState = BlendState.Opaque;
        graphicsDevice.DepthStencilState = DepthStencilState.None;
        graphicsDevice.SetRenderTargets(_deferredRenderTargetBindings);
        graphicsDevice.Clear(Color.Black);

        graphicsDevice.SetVertexBuffer(_fullScreenQuad);
        GameShaders.Deferred.CurrentTechnique = GameShaders.Deferred.Techniques["ClearDeferredBuffers"];
        GameShaders.Deferred.CurrentTechnique.Passes[0].Apply();
        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

        GameShaders.Deferred.CurrentTechnique = GameShaders.Deferred.Techniques["DrawDeferredBuffers"];
        graphicsDevice.DepthStencilState = DepthStencilState.Default;
        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

        foreach (var entity in entities)
        {
            entity.Draw(graphicsDevice, renderContext, GameShaders.Deferred);
        }

        if (GameShaders.PbrDeferred.CurrentTechnique != GameShaders.PbrDeferred.Techniques["Draw"])
            graphicsDevice.DepthStencilState = DepthStencilState.None;

        graphicsDevice.SetRenderTarget(null);
        graphicsDevice.Clear(Color.Black);
        GameShaders.PbrDeferred.ApplyRenderContext(Matrix.Identity, renderContext, default);
        graphicsDevice.SetVertexBuffer(_fullScreenQuad);

        GameShaders.PbrDeferred.CurrentTechnique.Passes[0].Apply();
        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
    }
}
