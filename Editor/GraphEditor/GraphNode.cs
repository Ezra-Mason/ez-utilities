using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ezutils.Editor
{
    public class GraphNode : IGraphElement, ISelectable, IGraphNode
    {
        
        public Rect Rect { get; set; }
        public string Header { get; set; }
        protected Rect _headerRect;
        protected Rect _contentRect;
        public int id;

        protected float _headerHeight = EditorGUIUtility.standardVerticalSpacing;
        protected float _bodyHeight = EditorGUIUtility.standardVerticalSpacing * 2f + EditorGUIUtility.singleLineHeight * 4f;
        protected float _bodyWidth = 200f;
        public virtual GUIStyle Style { get => GUI.skin.window; protected set => Style = value; }
        protected Vector2 _position;
        protected bool _beingDragged;

        //sockets
        public NodeSocket InSocket { get; set; }
        public NodeSocket OutSocket { get; set; }
        private GUIStyle _inSocketStyle = new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D,
            },
            active =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D
            },
            border = new RectOffset(4, 4, 12, 12)
        };
        private GUIStyle _outSocketStyle = new GUIStyle
        {
            normal =
            {
                //background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D,
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/objectfieldminithumb normal.png") as Texture2D,
            },
            active =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D
            },
            border = new RectOffset(4, 4, 12, 12)
        };

        public bool IsSelected { get; private set; }

        private Action<NodeSocket> _onClickIn;
        private Action<NodeSocket> _onClickOut;
        private Action<GraphNode> _onRemove;

        private GenericMenu _contextMenu;
        public GraphNode(
            Action<NodeSocket> onClickIn,
            Action<NodeSocket> onClickOut,
            Action<GraphNode> onClickRemove)
        {
            Rect = new Rect(0f, 0f, _bodyWidth, _bodyHeight);
            _headerRect = new Rect(0f, 0f, _bodyWidth, _headerHeight);
            //_activeStyle = _defaultStyle;
            InSocket = new NodeSocket(SocketType.IN, this, onClickIn);
            OutSocket = new NodeSocket(SocketType.OUT, this, onClickOut);
            _onRemove = onClickRemove;

            _contextMenu = new GenericMenu();
            _contextMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemove);
            //id = id
        }

        public virtual void Move(Vector2 delta)
        {
            Rect = new Rect(Rect.position.x + delta.x ,Rect.position.y + delta.y, _bodyWidth, _bodyHeight);
            _headerRect.position += delta;
        }

        public virtual void DrawElement()
        {
            //InSocket?.DrawElement();
            //OutSocket?.DrawElement();
            GUILayout.Label(Header, EditorStyles.boldLabel);
        }
        /// <summary>
        /// Process input events
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Should the GUI be repainted</returns>
        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    //lmb
                    if (e.button == 0)
                    {

                        if (Rect.Contains(e.mousePosition))
                        {
                            _beingDragged = true;
                            Select();
                        }
                        else
                        {
                            Deselct();
                        }
                        GUI.changed = true;
                    }
                    //rmb
                    if (e.button == 1 && IsSelected && Rect.Contains(e.mousePosition))
                    {
                        ShowContextMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    _beingDragged = false;
                    break;

                case EventType.MouseDrag:
                    //when dragging the mouse move this node and use the event to prevent its use by others
                    if (e.button == 0 && _beingDragged)
                    {
                        Move(e.delta);
                        e.Use();
                        // the gui should be repainted now that the node has moved
                        return true;
                    }
                    break;

                default:
                    break;
            }
            return false;
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselct()
        {
            IsSelected = true;
        }
        /// <summary>
        /// Show the <see cref="UnityEditor.GenericMenu"/> when right clicking on a node
        /// </summary>
        protected virtual void ShowContextMenu()
        {
            _contextMenu.ShowAsContext();
        }
        private void OnClickRemove()
        {
            if (_onRemove == null) return;
            Debug.Log($"onclickRemove");
            _onRemove(this);
        }
    }
}
