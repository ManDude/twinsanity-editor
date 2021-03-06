﻿using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TwinsaityEditor.Viewers
{
    public abstract class ThreeDViewer : GLControl
    {
        private Vector3 pos, rot, sca;
        private float range;
        private Timer input, refresh;
        private bool k_w, k_a, k_s, k_d, k_e, k_q, m_l, m_r;
        private int m_x, m_y;

        public ThreeDViewer()
        {
            pos = new Vector3(0, 0, 0);
            rot = new Vector3(0, 0, 0);
            sca = new Vector3(1.0f, 1.0f, 1.0f);
            range = 50f;
            input = new Timer
            {
                Interval = 16,
                Enabled = true
            };
            input.Tick += delegate (object sender, EventArgs e)
            {
                float speed = range / 100;
                int v = 0, h = 0, d = 0;
                if (k_w)
                    d++;
                if (k_a)
                    h++;
                if (k_s)
                    d--;
                if (k_d)
                    h--;
                if (k_e)
                    v++;
                if (k_q)
                    v--;
                Vector3 delta = new Vector3(h, v, d) * speed;
                Matrix4 rot_matrix = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), rot.X);
                rot_matrix *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), rot.Y);
                rot_matrix *= Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), rot.Z);

                Vector3 fin_delta = new Vector3(rot_matrix * new Vector4(delta, 1.0f));

                pos -= fin_delta;

                if ((h | v | d) != 0)
                    Invalidate();
            };
            refresh = new Timer
            {
                Interval = 100,
                Enabled = true
            };
            refresh.Tick += delegate (object sender, EventArgs e)
            {
                Invalidate();
            };
        }

        protected abstract void RenderObjects();

        private void ResetCamera()
        {
            pos = new Vector3(0, 0, 0);
            rot = new Vector3(0, 0, 0);
            Matrix4 view = Matrix4.Identity;
            Matrix4 proj = Matrix4.Identity;
            Matrix4 rot_matrix = Matrix4.Identity;//Matrix4.RotationYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

            //Create rotation matrix
            rot_matrix *= Matrix4.CreateRotationX(rot.X);
            rot_matrix *= Matrix4.CreateRotationY(rot.Y);
            rot_matrix *= Matrix4.CreateRotationZ(rot.Z);

            //Setup view and projection matrix
            Vector4 rot_vector = Vector4.Transform(new Vector4(0, 0, 1, 1), rot_matrix);
            view = Matrix4.LookAt(pos, new Vector3(pos.X + rot_vector.X, pos.Y + rot_vector.Y, pos.Z + rot_vector.Z), new Vector3(0, 1, 0));
            proj = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)this.Width / this.Height, 1.0f, 10000.0f);

            //Apply the matrices
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref view);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_l = true;
                    break;
                case MouseButtons.Right:
                    m_r = true;
                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_l = false;
                    break;
                case MouseButtons.Right:
                    m_r = false;
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (m_l)
            {
                rot.X += (e.X - m_x) / 180.0f * (float)Math.PI;
                rot.Y += (e.Y - m_y) / 180.0f * (float)Math.PI;
                rot.X += rot.X > (float)Math.PI ? -2*(float)Math.PI : rot.X < -(float)Math.PI ? +2*(float)Math.PI : 0;
                if (rot.Y > (float)Math.PI / 2)
                    rot.Y = (float)Math.PI / 2;
                if (rot.Y < (float)-Math.PI / 2)
                    rot.Y = (float)-Math.PI / 2;
                Invalidate();
            }
            m_x = e.X;
            m_y = e.Y;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            range += e.Delta * 5f;
            if (range > 100f)
                range = 100f;
            else if (range < 25f)
                range = 25f;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.W:
                case Keys.A:
                case Keys.S:
                case Keys.D:
                case Keys.Q:
                case Keys.E:
                case Keys.R:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.W:
                    k_w = true;
                    break;
                case Keys.A:
                    k_a = true;
                    break;
                case Keys.S:
                    k_s = true;
                    break;
                case Keys.D:
                    k_d = true;
                    break;
                case Keys.Q:
                    k_q = true;
                    break;
                case Keys.E:
                    k_e = true;
                    break;
                case Keys.R:
                    ResetCamera();
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                case Keys.W:
                    k_w = false;
                    break;
                case Keys.A:
                    k_a = false;
                    break;
                case Keys.S:
                    k_s = false;
                    break;
                case Keys.D:
                    k_d = false;
                    break;
                case Keys.Q:
                    k_q = false;
                    break;
                case Keys.E:
                    k_e = false;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            MakeCurrent();
            GL.Viewport(Location, Size);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Frustum(-0.5, +0.5, -0.5, +0.5, 0.5, 1000.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Scale(sca);
            GL.Rotate(rot.Y * 180 / Math.PI, 1, 0, 0);
            GL.Rotate(rot.X * 180 / Math.PI, 0, 1, 0);
            //Vector3 delta = new Vector3(-1, -1, -1) * range;
            //Matrix4 rot_matrix = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), rot.X);
            //rot_matrix *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), rot.Y);
            //rot_matrix *= Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), rot.Z);

            //Vector3 fin_delta = new Vector3(rot_matrix * new Vector4(delta, 1.0f));
            GL.Translate(-pos);
            //GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1f, 1f, 1f, 1f });
            //GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0, 0, 1f, 0 });
            //GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, new float[] { 0.2f, 0.2f, 0.2f });
            RenderObjects();
            SwapBuffers();
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.AlphaTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.AlphaFunc(AlphaFunction.Greater, 0);
            GL.ClearColor(0, 0, 1.0f, 0);
            //GL.Enable(EnableCap.ColorMaterial);
            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            input.Dispose();
            refresh.Dispose();
            base.Dispose(disposing);
        }
    }
}
