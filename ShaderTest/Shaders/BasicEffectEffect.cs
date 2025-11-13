using ShaderTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders;

public class BasicEffectEffect : BaseEffect
{
    public BasicEffectEffect(Effect cloneSource) : base(cloneSource) { }

    public override void ApplyRenderContext(Matrix world, RenderContext renderContext, Material material)
    {
        Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
        Parameters["WorldViewProj"].SetValue(world * renderContext.View * renderContext.Projection);
        Parameters["Texture"].SetValue(material.Texture);
    }
}
