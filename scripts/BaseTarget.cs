using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheShooter.scripts
{
    public partial class BaseTarget : StaticBody3D
    {
        [Export]
        public CollisionShape3D Collision { get; set; }
        [Export]
        public AnimationPlayer Animation { get; set; }

        public bool EnabledToShot { get; set; } = true;

        public virtual void DetectShoot()
        {
            EnabledToShot = false;
            if (!Animation.IsPlaying())
            {
                Animation.Play("rotate");
            }
            EnabledToShot = true;
        }

        public override void _Ready()
        {
        }
    }
}
