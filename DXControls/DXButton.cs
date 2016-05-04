﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rampastring.Tools;
using System;

namespace Rampastring.XNAUI.DXControls
{
    /// <summary>
    /// A basic button.
    /// </summary>
    public class DXButton : DXControl
    {
        public DXButton(WindowManager windowManager) : base(windowManager) 
        {
            AlphaRate = UISettings.DefaultAlphaRate;
            TextColorIdle = UISettings.ButtonColor;
            TextColorHover = UISettings.ButtonHoverColor;
        }

        public Texture2D IdleTexture { get; set; }

        public Texture2D HoverTexture { get; set; }

        public SoundEffect HoverSoundEffect { get; set; }
        SoundEffectInstance hoverSoundInstance;

        public SoundEffect ClickSoundEffect { get; set; }
        SoundEffectInstance clickSoundInstance { get; set; }

        public float AlphaRate { get; set; }
        float idleTextureAlpha = 1.0f;
        float hoverTextureAlpha = 0.0f;
        ButtonAnimationMode animationMode;

        public Keys HotKey { get; set; }

        public int FontIndex { get; set; }

        bool allowClick = true;
        public bool AllowClick
        {
            get { return allowClick; }
            set { allowClick = value; }
        }

        string _text = String.Empty;
        public override string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (adaptiveText)
                {
                    Vector2 textSize = Renderer.GetTextDimensions(_text, FontIndex);
                    if (textSize.X < ClientRectangle.Width)
                    {
                        TextXPosition = (int)((ClientRectangle.Width - textSize.X) / 2);
                    }
                    else if (textSize.X > ClientRectangle.Width)
                    {
                        TextXPosition = (int)((textSize.X - ClientRectangle.Width) / -2);
                    }

                    if (textSize.Y < ClientRectangle.Height)
                    {
                        TextYPosition = (int)((ClientRectangle.Height - textSize.Y) / 2);
                    }
                    else if (textSize.Y > ClientRectangle.Height)
                    {
                        TextYPosition = Convert.ToInt32((textSize.Y - ClientRectangle.Height) / -2);
                    }
                }
            }
        }

        public int TextXPosition { get; set; }
        public int TextYPosition { get; set; }

        public Color TextColorIdle { get; set; }

        public Color TextColorHover { get; set; }

        Color textColor = Color.White;

        bool adaptiveText = true;
        public bool AdaptiveText
        {
            get { return adaptiveText; }
            set { adaptiveText = value; }
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (!AllowClick)
                return;

            animationMode = ButtonAnimationMode.HIGHLIGHT;
            idleTextureAlpha = 0.5f;
            hoverTextureAlpha = 0.75f;
            textColor = TextColorHover;

#if !WINDOWSGL
            if (HoverSoundEffect != null)
            {
                hoverSoundInstance.Play();
            }
#endif
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (!AllowClick)
                return;

            animationMode = ButtonAnimationMode.RETURN;
            idleTextureAlpha = 0.75f;
            hoverTextureAlpha = 0.5f;
            textColor = TextColorIdle;
        }

        public override void OnLeftClick()
        {
            if (!AllowClick)
                return;

#if !WINDOWSGL
            if (ClickSoundEffect != null)
                clickSoundInstance.Play();
#endif

            base.OnLeftClick();
        }

        public override void Initialize()
        {
            if (HoverSoundEffect != null)
                hoverSoundInstance = HoverSoundEffect.CreateInstance();

            if (ClickSoundEffect != null)
                clickSoundInstance = ClickSoundEffect.CreateInstance();

            if (IdleTexture != null)
            {
                ClientRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y,
                    IdleTexture.Width, IdleTexture.Height);
            }

            textColor = TextColorIdle;
        }

        protected override void ParseAttributeFromINI(IniFile iniFile, string key, string value)
        {
            switch (key)
            {
                case "TextColorIdle":
                    TextColorIdle = AssetLoader.GetColorFromString(value);
                    textColor = TextColorIdle;
                    return;
                case "TextColorHover":
                    TextColorHover = AssetLoader.GetColorFromString(value);
                    return;
                case "HoverSoundEffect":
                    HoverSoundEffect = AssetLoader.LoadSound(value);
                    return;
                case "ClickSoundEffect":
                    ClickSoundEffect = AssetLoader.LoadSound(value);
                    return;
                case "AdaptiveText":
                    AdaptiveText = Utilities.BooleanFromString(value, true);
                    return;
                case "AlphaRate":
                    AlphaRate = Utilities.FloatFromString(value, 0.01f);
                    return;
                case "FontIndex":
                    FontIndex = Utilities.IntFromString(value, 0);
                    if (adaptiveText)
                        Text = _text;
                    return;
                case "IdleTexture":
                    IdleTexture = AssetLoader.LoadTexture(iniFile.GetStringValue(Name, "IdleTexture", String.Empty));
                    ClientRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y,
                        IdleTexture.Width, IdleTexture.Height);
                    if (adaptiveText)
                        Text = _text;
                    return;
                case "HoverTexture":
                    HoverTexture = AssetLoader.LoadTexture(iniFile.GetStringValue(Name, "HoverTexture", String.Empty));
                    return;
            }

            base.ParseAttributeFromINI(iniFile, key, value);
        }

        public override void Kill()
        {
            base.Kill();

            if (IdleTexture != null)
                IdleTexture.Dispose();

            if (HoverTexture != null)
                HoverTexture.Dispose();

            if (HoverSoundEffect != null)
                HoverSoundEffect.Dispose();

            if (ClickSoundEffect != null)
                ClickSoundEffect.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (animationMode == ButtonAnimationMode.HIGHLIGHT)
            {
                idleTextureAlpha -= AlphaRate;
                if (idleTextureAlpha < 0.0f)
                {
                    idleTextureAlpha = 0.0f;
                }

                hoverTextureAlpha += AlphaRate;
                if (hoverTextureAlpha >= 1.0f)
                {
                    hoverTextureAlpha = 1.0f;
                }
            }
            else
            {
                hoverTextureAlpha -= AlphaRate;
                if (hoverTextureAlpha < 0.0f)
                {
                    hoverTextureAlpha = 0.0f;
                }

                idleTextureAlpha += AlphaRate;
                if (idleTextureAlpha >= 1.0f)
                {
                    idleTextureAlpha = 1.0f;
                }
            }

            if (Parent != null && Parent.IsActive && Keyboard.PressedKeys.Contains(HotKey))
                OnLeftClick();
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle windowRectangle = WindowRectangle();

            if (IdleTexture != null)
            {
                if (idleTextureAlpha > 0f)
                    Renderer.DrawTexture(IdleTexture, windowRectangle, 
                        new Color(RemapColor.R, RemapColor.G, RemapColor.B, (int)(idleTextureAlpha * Alpha * 255)));

                if (HoverTexture != null && hoverTextureAlpha > 0f)
                    Renderer.DrawTexture(HoverTexture, windowRectangle, 
                        new Color(RemapColor.R, RemapColor.G, RemapColor.B, (int)(hoverTextureAlpha * Alpha * 255)));
            }

            Vector2 textPosition = new Vector2(windowRectangle.X + TextXPosition, windowRectangle.Y + TextYPosition);

            if (!Enabled || !AllowClick)
                Renderer.DrawStringWithShadow(_text, FontIndex, textPosition, GetColorWithAlpha(Color.DarkGray));
            else
                Renderer.DrawStringWithShadow(_text, FontIndex, textPosition, GetColorWithAlpha(textColor));

            base.Draw(gameTime);
        }
    }

    enum ButtonAnimationMode
    {
        NONE,
        HIGHLIGHT,
        RETURN
    }
}
