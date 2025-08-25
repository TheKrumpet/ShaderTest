using Microsoft.Xna.Framework.Graphics;
using ShaderTest.Entities;
using ShaderTest.Extensions;
using ShaderTest.Renderer;
using ShaderTest.Shaders;
using ShaderTest.UI;
using ShaderTest.Updatables;

namespace ShaderTest
{
    public class ShaderTestGame : Game
    {
        public Camera Camera { get; private set; }
        public MouseInputHandler Mouse { get; private set; }
        public List<ModelEntity> Entities { get; private set; }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _arial;

        private RenderTarget2D _shadowMap;
        private Texture2D _pixel;
        private GameStats _stats;
        private IRenderer[] _renderers;
        private Skybox _skybox;
        private Sun _sun;
        private List<Updatable> _updatable;
        private UiWindow _uiWindow;
        private RenderControl _render;
        private IRenderer _currentRenderer;

        public ShaderTestGame()
        {
            _graphics = new(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            _graphics.HardwareModeSwitch = false;
            _graphics.PreferMultiSampling = false;
            _graphics.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = false;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            _uiWindow = new UiWindow(this);
            _stats = new GameStats(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Entities = [];
            _updatable = [];

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameShaders.Initialise(GraphicsDevice, Content);

            Camera = new Camera(this);
            _updatable.Add(Camera);
            _uiWindow.AddToTab("Render", Camera);

            _arial = Content.Load<SpriteFont>("Arial");
            _shadowMap = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents)
            {
                Name = "ShadowMap"
            };

            GameShaders.Pbr.ShadowMap = _shadowMap;
            GameShaders.PbrDeferred.ShadowMap = _shadowMap;
            var entityFactory = new EntityFactory(Content);

            Entities.Add(entityFactory.CreateEntity<GroundEntity>("Ground"));
            Entities.Add(entityFactory.CreateEntity<CampfireEntity>("Campfire"));
            Entities.Add(entityFactory.CreateEntity<CarEntity>("Car"));
            Entities.Add(entityFactory.CreateEntity<TentEntity>("Tent"));

            _skybox = new Skybox();
            _skybox.Initialise(GraphicsDevice);
            _uiWindow.AddToTab("Render", _skybox);

            _sun = new Sun(this);
            _updatable.Add(_sun);
            _uiWindow.AddToTab("Render", _sun);

            Mouse = new MouseInputHandler(this);
            _updatable.Add(Mouse);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData([Color.White]);

            var editor = new EntityEdit(this);

            _updatable.Add(editor);
            _uiWindow.AddToTab("Entity", editor);

            _render = new RenderControl();
            _uiWindow.AddToTab("Render", _render);

            _renderers = [
                new PbrDeferredRenderer(),
                new PbrForwardRenderer(),
                new QuadTestRenderer(),
                new BasicEffectRenderer()
            ];

            foreach (var renderer in _renderers)
            {
                renderer.Initialise(Content, GraphicsDevice);
            }

            var pbrDeferredRenderer = _renderers.First(r => r is PbrDeferredRenderer) as PbrDeferredRenderer;

            var loadedTextures = Content.GetLoaded<Texture2D>();
            McFaceImGui.Initialise([
                pbrDeferredRenderer.AlbedoMap,
                pbrDeferredRenderer.NormalMap,
                pbrDeferredRenderer.DepthMap,
                pbrDeferredRenderer.PbrMap,
                _shadowMap,
                .. loadedTextures
            ]);

            var textureView = new TextureView(_uiWindow.Renderer);
            _uiWindow.AddToTab("Textures", textureView);

            GameDebug.Initialize(GraphicsDevice, _arial);

            _currentRenderer = _renderers.First(r => r is PbrForwardRenderer);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var updateable in _updatable)
            {
                updateable.Update(gameTime);
            }

            foreach (var entity in Entities)
            {
                entity.Update(gameTime);
            }

            _stats.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            // Shadow map
            GraphicsDevice.SetRenderTarget(_shadowMap);
            GraphicsDevice.Clear(Color.White);

            var renderContext = new RenderContext(Camera, _sun);

            foreach (var entity in Entities)
            {
                if (!entity.IncludeInShadowMap) continue;

                entity.Draw(GraphicsDevice, renderContext, GameShaders.ShadowMap);
            }

            _spriteBatch.Begin();
            _currentRenderer.Render(GraphicsDevice, renderContext, _spriteBatch, Entities);
            //_stats.Draw(gameTime, _spriteBatch, GraphicsDevice, _arial);
            _spriteBatch.End();

            _uiWindow.RenderUi(gameTime);


            //base.Draw(gameTime);
        }
    }
}
