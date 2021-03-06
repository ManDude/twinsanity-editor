﻿using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Twinsanity;

namespace TwinsaityEditor.Viewers
{
    public partial class SMViewer : ThreeDViewer
    {
        private int displaylist;
        //private List<Model> data;
        private Color[] colors = new[] { Color.Gray, Color.Green, Color.Red, Color.DarkBlue, Color.Yellow, Color.Pink, Color.DarkCyan, Color.DarkGreen, Color.DarkRed, Color.Brown, Color.DarkMagenta, Color.Orange, Color.DarkSeaGreen, Color.Bisque, Color.Coral };
        //private int displaylist;

        public SMViewer()//Model data)
        {
            //initialize variables here
            displaylist = -1;
            //this.data = new List<Model> { data };
        }

        protected override void RenderObjects()
        {
            //put all object rendering code here
            if (displaylist == -1) //if model(s) display list is non-existant
            {
                displaylist = GL.GenLists(1);
                GL.NewList(displaylist, ListMode.CompileAndExecute);
                GL.Begin(PrimitiveType.Triangles);
                //foreach (Model mdl in data)
                {

                }
                GL.End();
                GL.EndList();
            }
            else
                GL.CallList(displaylist);
            //throw new NotImplementedException();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (displaylist != -1)
            {
                GL.DeleteLists(displaylist, 1);
                displaylist = -1;
            }
            base.Dispose(disposing);
        }
    }
}
