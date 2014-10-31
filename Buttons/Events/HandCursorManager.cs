﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using KinectFittingRoom.Events;

namespace KinectFittingRoom.Buttons.Events
{
    public class HandCursorManager
    {
        #region Variables
        /// <summary>
        /// Parent canvas for all buttons
        /// </summary>
        private readonly Canvas m_canvas;
        /// <summary>
        /// Last element hit by cursor
        /// </summary>
        private UIElement m_lastElement;
        /// <summary>
        /// Prevents from reinitializing the singleton
        /// </summary>
        private static bool m_isInitialized;
        #endregion Variables

        #region Properties
        /// <summary>
        /// Hand cursor manager instance
        /// </summary>
        public static HandCursorManager Instance { get; private set; }
        #endregion Properties

        private HandCursorManager(Canvas canvas)
        {
            m_canvas = canvas;
        }

        #region Methods
        /// <summary>
        /// Creates the instance of HandCursorManager
        /// </summary>
        /// <param name="canvas">Parent canvas</param>
        public static void Create(Canvas canvas)
        {
            if (!m_isInitialized)
            {
                Instance = new HandCursorManager(canvas);
                m_isInitialized = true;
            }
        }

        /// <summary>
        /// Handles hand cursor events
        /// </summary>
        /// <param name="handCursor">Canvas with hand cursor</param>
        /// <param name="leftHand">Left hand joint</param>
        /// <param name="leftMappedHand">Left hand joint mapped to screen coordinates</param>
        /// <param name="rightHand">Right hand joint</param>
        /// <param name="rightMappedHand">Right hand joint mapped to screen coordinates</param>
        public void HandleHandCursorEvents(Canvas handCursor, Vector3D leftHand, Point leftMappedHand, Vector3D rightHand, Point rightMappedHand)
        {
            UIElement element = GetElementAtPoint(new Point(leftHand.X, leftHand.Y), m_canvas);
            Vector3D activeHand = leftHand;
            Point activeMappedHand = leftMappedHand;
            handCursor.Visibility = Visibility.Collapsed;

            if (element == null)
            {
                element = GetElementAtPoint(new Point(rightHand.X, rightHand.Y), m_canvas);
                activeHand = rightHand;
                activeMappedHand = rightMappedHand;
            }

            if (element != null)
            {
                handCursor.Visibility = Visibility.Visible;
                Canvas.SetLeft(handCursor, activeMappedHand.X - (handCursor.ActualWidth / 2.0));
                Canvas.SetTop(handCursor, activeMappedHand.Y - (handCursor.Height / 2.0));

                element.RaiseEvent(new HandCursorEventArgs(KinectInput.HandCursorMoveEvent, activeHand));
                if (element != m_lastElement)
                {
                    if (m_lastElement != null)
                        m_lastElement.RaiseEvent(new HandCursorEventArgs(KinectInput.HandCursorLeaveEvent, activeHand));
                    element.RaiseEvent(new HandCursorEventArgs(KinectInput.HandCursorEnterEvent, activeHand));
                }
            }
            m_lastElement = element;
        }

        /// <summary>
        /// Searches for the element in canvas at certain point
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="canvas">Canvas</param>
        /// <returns>Element in canvas at point</returns>
        private static UIElement GetElementAtPoint(Point point, Canvas canvas)
        {
            if (!canvas.IsVisible)
                return null;

            Point canvasPoint = canvas.PointFromScreen(point);
            IInputElement element = canvas.InputHitTest(canvasPoint);

            if (element is UIElement)
                return (UIElement)element;
            return null;
        }
        #endregion Methods
    }
}