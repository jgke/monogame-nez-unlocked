using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Sprites;

namespace MyGame;

public class Ball : Component, IUpdatable
{
    public float MoveSpeed = 150;
    Vector2 _velocity;

    VirtualIntegerAxis _xAxisInput;
    VirtualIntegerAxis _yAxisInput;

    public override void OnAddedToEntity()
    {
        var texture = Entity.Scene.Content.LoadTexture("ball");

        Entity.AddComponent(new SpriteRenderer(texture));

        SetupInput();
    }

    public override void OnRemovedFromEntity()
    {
        // deregister virtual input
        _xAxisInput.Deregister();
        _yAxisInput.Deregister();
    }

    void SetupInput()
    {
        _xAxisInput = new VirtualIntegerAxis();
        _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
        _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
        _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

        _yAxisInput = new VirtualIntegerAxis();
        _yAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadUpDown());
        _yAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickY());
        _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));
    }

    void IUpdatable.FixedUpdate()
    {
        // handle movement and animations
        var moveDir = new Vector2(_xAxisInput.Value, _yAxisInput.Value);
        string animation = null;

        _velocity.X = moveDir.X switch
        {
            < 0 => -MoveSpeed,
            > 0 => MoveSpeed,
            _ => 0
        };

        _velocity.Y = moveDir.Y switch
        {
            < 0 => -MoveSpeed,
            > 0 => MoveSpeed,
            _ => 0
        };

        Entity.Transform.Position += _velocity;
    }

    public void DrawUpdate() { }
}
public class DefaultScene : Scene
{
    public override void Initialize()
    {
        base.Initialize();

        SetDesignResolution(800, 600, SceneResolutionPolicy.ShowAllPixelPerfect);
        Screen.SetSize(800 * 2, 600 * 2);

        var ballEntity = CreateEntity("ball", new Vector2(400, 300));
        ballEntity.AddComponent(new Ball());
    }
}
