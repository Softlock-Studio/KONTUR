using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI 
{
    [Serializable]
    public enum CursorState
    { 
        Default,
        Hover
    }

    //TODO get rid of singletone and put into DI
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] List<CursorTextureTuple> cursors;

        public static CursorManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ChangeCursor(CursorState.Default);
        }

        public void ChangeCursor(CursorState state)
        {
            foreach (var pair in cursors)
            { 
                if (pair._state == state)
                    Cursor.SetCursor(pair._texture, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    [Serializable]
    public class CursorTextureTuple
    {
        public CursorState _state;
        public Texture2D _texture;
    }
}