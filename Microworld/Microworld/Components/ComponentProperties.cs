using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Components.Properties
{
    public delegate void OnRecievedEventHandler(bool correct);
    //public delegate void OnLiquidVolumeChangedEventHandler(ILiquidContainer sender, int prevVolume, int newVolume);

    public interface IDrawBorder
    {
    }

    public interface IContainer
    {
    }

    public interface ILightEmitting
    {
        float GetBrightness(float x, float y);
        bool IsInRange(Component c);
    }

    public interface IMagnetic
    {
        Vector2 GetFieldForce(float x, float y);
        void SetRange(Vector2 f);
        Vector2 GetRange();
        bool IsInRange(Component c);
        float GetRadius();
        int GetPolarity();
        void SetPolarity(MagnetPole p);
    }

    public interface IDragDropPlacable
    {
        bool CanStart(int x, int y);
        void DrawGhost(MicroWorld.Graphics.Renderer renderer, int x1, int y1, int x2, int y2);
        bool CanEnd(int x1, int y1, int x2, int y2);
        void Place(int x1, int y1, int x2, int y2);
    }

    public interface IRotatable
    {
        void Rotate(Vector2 origin, float deltaAngle);
        void ResetPosition();
        void Connect(Component component, RotatableConnector connector);
        bool CanConnect(Component component);
        void Disconnect();
        RotatableConnector GetConnector();
    }

    public interface IRotator
    {
        void Rotate(float deltaAngle);
        void Connect(Component component, RotatableConnector connector);
        bool CanConnect(Component component);
        void DrawTrajectory(MicroWorld.Graphics.Renderer renderer);
        void Disconnect();
        RotatableConnector GetConnector();
    }

    public interface IMovable
    {
        void Move(Vector2 delta);
        void ResetPosition();
    }

    public interface IMover
    {
        void DrawTrajectory(MicroWorld.Graphics.Renderer renderer);
    }

    public interface ICore
    {
        event OnRecievedEventHandler onFinishedRecieving;
        bool IsCorrect();
        void InvokeRecievedFinished();
    }

    public interface IHasMapToolTip
    {
        /// <summary>
        /// Returns what to display on MapOverlay ToolTip, when activated from Overlay HUD (if c == null), or from component otherwise.
        /// </summary>
        /// <returns></returns>
        String GetMapOverlayToolTip(Component c = null);
    }

    public interface IUsesLight : IHasMapToolTip
    {
        /// <summary>
        /// Returns color of line, that goes from c to this component. Usually is set to return white color with modified opacity.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        Color GetMapLineColor(ILightEmitting c = null);
    }

    public interface IUsesMagnetism : IHasMapToolTip
    {
        /// <summary>
        /// Returns color of line, that goes from c to this component. Usually is set to return red/blue color with modified opacity.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        float GetMapLineOpacity(IMagnetic c = null);
    }

    public interface IAttractsLightning
    {
        void GetStruck(Component origin, float voltage, int time);
    }

    public interface ICollidable
    {
        Colliders.ColliderGroup ColliderGroup { get; }
        void RegisterColliders();
        void UnRegisterColliders();
    }

    public interface IUnselecrable
    {
    }

    public interface IResizable
    {
        Direction CanResize(Vector2 pos);
        void OnResizeStart(Direction point, Vector2 clickPoint);
        void Resize(Direction point, float dx, float dy);
        void OnResizeFinished(Direction point);
        void CancelResize();
    }

    public interface IAffectedByEMP
    {
        void TouchedByEMP(Vector2 EMPCenter);
    }

    public interface ICreatesPressure
    {
        float GetPressure();
        void InitSpreadPressure();
    }

    public interface IRequiresCircuitRecalculation
    {
        void PreIndividualUpdate();
        void PostIndividualUpdate();
        int GetPriority();
    }
}
