using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Surfaces;

namespace WoMSadGui.Specific
{
    public class MogwaiEntity : Entity
    {
        private Animated _defAnimated;

        private Entity _animEntity;

        //public MogwaiEntity(int glyph, Color color, Font font) : base(1,1)
        public MogwaiEntity(Animated animated) : base(animated)
        {
            _defAnimated = animated;

            // animation entity
            _animEntity = new Entity(new Animated("default", 1, 1, font));

            // add animation entity
            Children.Add(_animEntity);

            //_animEntity.AnimationStateChanged += AnimationChanged;
        }

        public void Init()
        {
            AddAnimation("dead", 143, Color.Gainsboro);
            AddLayeredAnimation("damage", 15, Color.Red);
        }

        private void AnimationChanged(object sender, Animated.AnimationStateChangedEventArgs e)
        {
            var entity = sender as Entity;

            if (entity.Animation.Name != "default" && e.NewState == Animated.AnimationState.Finished)
            {
                entity.Animation = entity.Animations["default"];
            }
        }

        public void AddAnimation(string name, int glyph, Color color)
        {
            var newAnim = new Animated(name, 1, 1, font)
            {
                AnimationDuration = 1
            };
            var frame = newAnim.CreateFrame();
            frame[0].Glyph = glyph;
            frame[0].Foreground = color;

            Animations.Add(name, newAnim);
        }

        public void AddLayeredAnimation(string name, int glyph, Color color)
        {
            var newAnim = new Animated(name, 1, 1, font)
            {
                AnimationDuration = 1
            };
            var frame = newAnim.CreateFrame();
            newAnim.CreateFrame();
            frame[0].Glyph = glyph;
            frame[0].Foreground = color;

            _animEntity.Animations.Add(name, newAnim);
        }

        public void Dead()
        {
            //Animation = Animations["dead"];
            //Animation.Restart();

            var animated = new Animated("default", 1, 1, font);
            var frame = animated.CreateFrame();
            frame[0].Glyph = 143;
            frame[0].Foreground = Color.Red;
            Animation = animated;
        }

        public void Restart(string name)
        {
            _animEntity.Animation = _animEntity.Animations[name];
            _animEntity.Animation.Restart();
        }

        internal void Looted()
        {
            var animated = new Animated("default", 1, 1, font);
            var frame = animated.CreateFrame();
            frame[0].Glyph = 143;
            frame[0].Foreground = Color.DarkGray;
            Animation = animated;
        }
    }
}
