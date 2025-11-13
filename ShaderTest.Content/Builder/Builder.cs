/// <summary>
/// Entry point for the Content Builder project, 
/// which when executed will build content according to the "Content Collection Strategy" defined in the Builder class.
/// </summary>
/// <remarks>
/// Make sure to validate the directory paths in the "ContentBuilderParams" for your specific project.
/// For more details regarding the Content Builder, see the MonoGame documentation: <tbc.>
/// </remarks>

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Framework.Content.Pipeline.Builder;

var contentCollectionArgs = new ContentBuilderParams()
{
    Mode = ContentBuilderMode.Builder,
    WorkingDirectory = $"{AppContext.BaseDirectory}../../../", // path to where your content folder can be located
    SourceDirectory = "Assets", // Not actually needed as this is the default, but added for reference
    Platform = TargetPlatform.DesktopGL
};
var builder = new Builder();

if (args is not null && args.Length > 0)
{
    builder.Run(args);
}
else
{
    builder.Run(contentCollectionArgs);
}

return builder.FailedToBuild > 0 ? -1 : 0;

public class Builder : ContentBuilder
{
    public override IContentCollection GetContentCollection()
    {
        ContentCollection contentCollection = new();

        // Models
        ModelProcessor processor = new()
        {
            ColorKeyColor = Color.Magenta,
            ColorKeyEnabled = false,
            GenerateMipmaps = false,
            PremultiplyTextureAlpha = false,
            PremultiplyVertexColors = false,
            ResizeTexturesToPowerOfTwo = false,
            RotationX = 0,
            RotationY = 0,
            RotationZ = 0,
            Scale = 1,
            SwapWindingOrder = false,
            TextureFormat = TextureProcessorOutputFormat.Color,
            DefaultEffect = MaterialProcessorDefaultEffect.BasicEffect,
            GenerateTangentFrames = true,
        };

        contentCollection.Include<WildcardRule>("*.fbx", new FbxImporter(), processor);

        // Effects
        EffectProcessor effectProcessor = new()
        {
            DebugMode = EffectProcessorDebugMode.Debug
        };

        contentCollection.Include<WildcardRule>("*.fx", new EffectImporter(), effectProcessor);

        // Textures
        contentCollection.Include<WildcardRule>("*.png");
        contentCollection.Include<WildcardRule>("*.jpg");

        // Spritefonts
        contentCollection.Include<WildcardRule>("*.spritefont");

        return contentCollection;
    }
}