using Godot;

[GlobalClass]
public partial class PlayerMovementConfig : Resource 
{
    [Export]
    [ExportGroup("HeadShaking")]
    public float ShakingFrequency { get; set; } = 2.0f;
    [Export]
    public float ShakingAmplitude { get; set; } = 0.08f;
    [Export]
    [ExportGroup("Walk")]
    public float JumpVelocity { get; set; } = 4.5f;
    [Export]
    public float Speed { get; set; } = 5.0f;
    [Export]
    public float SprintSpeed { get; set; } = 10.0f;
    [Export]
    public float SneakSpeed { get; set; } = 2.3f;
    [Export]
    public float LerpSpeed { get; set; } = 7.0f;

    [Export]
    [ExportGroup("Camera")]
    public float MouseSensitivity { get; set; } = 0.3f;

    [Export]
    [ExportSubgroup("FOV")]
    public float BaseFOV { get; set; } = 75.0f;
    [Export]
    public float MaxFOV { get; set; } = 90.0f;
    [Export]
    public float MinFOV { get; set; } = 50.0f;
    public CharacterBody3D RootNode { get; set; }
}

