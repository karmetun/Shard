﻿/*
*
*   The baseline functionality for getting text to work via SDL.   You could write your own text 
*       implementation (and we did that earlier in the course), but bear in mind DisplaySDL is built
*       upon this class.
*   @author Michael Heron
*   @version 1.0
*   
*/

using SDL2;
using System;
using System.Collections.Generic;

namespace Shard
{

    // We'll be using SDL2 here to provide our underlying graphics system.
    // Decides
    class TextDetails
    {
        string text;
        double x, y;
        SDL.SDL_Color col;
        int size;
        IntPtr font;
        IntPtr lblText; // Surface used to draw.


        public TextDetails(string text, double x, double y, SDL.SDL_Color col, int spacing)
        {
            this.text = text;
            this.x = x;
            this.y = y;
            this.col = col;
            this.size = spacing;
        }

        public string Text
        {
            get => text;
            set => text = value;
        }
        public double X
        {
            get => x;
            set => x = value;
        }
        public double Y
        {
            get => y;
            set => y = value;
        }
        public SDL.SDL_Color Col
        {
            get => col;
            set => col = value;
        }
        public int Size
        {
            get => size;
            set => size = value;
        }
        public IntPtr Font { get => font; set => font = value; }
        public IntPtr LblText { get => lblText; set => lblText = value; }
    }

    // Handles the drawing of objects.
    class DisplayText : Display
    {
        protected IntPtr _window, _rend;
        uint _format;
        int _access;
        private List<TextDetails> myTexts; //Buffer of things to be drawn
        private Dictionary<string, IntPtr> fontLibrary;

        // Clears the buffer
        public override void clearDisplay()
        {
            foreach (TextDetails td in myTexts)
            {
                SDL.SDL_DestroyTexture(td.LblText);
            }

            myTexts.Clear();
            SDL.SDL_SetRenderDrawColor(_rend, 0, 0, 0, 255);
            SDL.SDL_RenderClear(_rend);

        }

        public IntPtr loadFont(string path, int size)
        {
            string key = path + "," + size;

            if (fontLibrary.ContainsKey(key))
            {
                return fontLibrary[key];
            }

            fontLibrary[key] = SDL_ttf.TTF_OpenFont(path, size);
            return fontLibrary[key];
        }

        private void update()
        {


        }

        // Draws everything
        private void draw()
        {

            //Draws each in turn
            foreach (TextDetails td in myTexts)
            {

                SDL.SDL_Rect sRect;

                sRect.x = (int)td.X;
                sRect.y = (int)td.Y;
                sRect.w = 0;
                sRect.h = 0;


                SDL_ttf.TTF_SizeText(td.Font, td.Text, out sRect.w, out sRect.h);
                SDL.SDL_RenderCopy(_rend, td.LblText, IntPtr.Zero, ref sRect);

            }

            // We are done, commit to screen. 
            SDL.SDL_RenderPresent(_rend);

        }

        public override void display()
        {

            update();
            draw();
        }

        public override void setFullscreen()
        {
            SDL.SDL_SetWindowFullscreen(_window,
                 (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
        }

        public override void initialize()
        {
            fontLibrary = new Dictionary<string, IntPtr>();

            setSize(1280, 864);

            SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            SDL_ttf.TTF_Init();
            _window = SDL.SDL_CreateWindow("Shard Game Engine",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                getWidth(),
                getHeight(),
                0);


            _rend = SDL.SDL_CreateRenderer(_window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);


            SDL.SDL_SetRenderDrawBlendMode(_rend, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            SDL.SDL_SetRenderDrawColor(_rend, 0, 0, 0, 255);


            myTexts = new List<TextDetails>();
        }



        public override void showText(string text, double x, double y, int size, int r, int g, int b)
        {
            int nx, ny, w = 0, h = 0;

            IntPtr font = loadFont("Fonts/calibri.ttf", size);
            SDL.SDL_Color col = new SDL.SDL_Color();

            col.r = (byte)r;
            col.g = (byte)g;
            col.b = (byte)b;
            col.a = (byte)255;

            if (font == IntPtr.Zero)
            {
                Debug.getInstance().log("TTF_OpenFont: " + SDL.SDL_GetError());
            }

            TextDetails td = new TextDetails(text, x, y, col, 12);

            td.Font = font;

            IntPtr surf = SDL_ttf.TTF_RenderText_Blended(td.Font, td.Text, td.Col);
            IntPtr lblText = SDL.SDL_CreateTextureFromSurface(_rend, surf);
            SDL.SDL_FreeSurface(surf);

            SDL.SDL_Rect sRect;

            sRect.x = (int)x;
            sRect.y = (int)y;
            sRect.w = w;
            sRect.h = h;

            SDL.SDL_QueryTexture(lblText, out _format, out _access, out sRect.w, out sRect.h);

            td.LblText = lblText;

            myTexts.Add(td);


        }
        public override void showText(char[,] text, double x, double y, int size, int r, int g, int b)
        {
            string str = "";
            int row = 0;

            for (int i = 0; i < text.GetLength(0); i++)
            {
                str = "";
                for (int j = 0; j < text.GetLength(1); j++)
                {
                    str += text[j, i];
                }


                showText(str, x, y + (row * size), size, r, g, b);
                row += 1;

            }

        }
    }
}
