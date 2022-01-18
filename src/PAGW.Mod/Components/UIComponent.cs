extern alias GameScripts;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PAGW.Mod.Components
{
	internal class UIComponent : MonoBehaviour
    {

        private const int MaxNotificationCount = 15;

        private readonly Queue<NotificationItem> notificationQueue = new Queue<NotificationItem>(24);

        private readonly string versionString = "Mod " + typeof(EntryPoint).Assembly.GetName().Version.ToString(2) +
#if DEBUG
            " (DEBUG)" +
#endif
            "\n" + ModController.Config.ActiveSettings;

        public bool DisplayVersion { get; set; }



        private void OnGUI()
        {
            Styles.Init();
            DrawVersionGUI();
            DrawNotificationGUI();
        }
        


        private void DrawVersionGUI()
        {
            if (DisplayVersion)
                GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), versionString, Styles.VersionLabel);
        }

        private void DrawNotificationGUI()
        {
            lock (notificationQueue)
            {
                var cutoffTime = Time.unscaledTime - ModController.Config.NotificationDisplayTime;

                while (notificationQueue.Count > 0 && notificationQueue.Peek().EnqueuedTime < cutoffTime)
                    notificationQueue.Dequeue();

                if (notificationQueue.Count > 0)
                {
                    var labelStyle = Styles.Notifications.Label;
                    GUILayout.BeginVertical(Styles.Notifications.Container);
                    foreach (var item in notificationQueue)
                    {
                        GUILayout.Label(item.Message, labelStyle, GUILayout.ExpandWidth(false));
                    }
                    GUILayout.EndVertical();
                }
            }
        }

        public void AddNotification(string message)
        {
            lock (notificationQueue)
            {
                while (notificationQueue.Count >= MaxNotificationCount)
                    notificationQueue.Dequeue();
                notificationQueue.Enqueue(new NotificationItem(message));
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private struct NotificationItem
        {
            public string Message { get; }
            public float EnqueuedTime { get; set; }

            public NotificationItem(string message)
            {
                Message = message;
                EnqueuedTime = Time.unscaledTime;
            }
        }

        private static class Styles
        {
            private static bool initialized;

            public static void Init()
            {
                if (initialized)
                    return;
                initialized = true;

                VersionLabel = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    normal = { textColor = Color.black },
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(10, 10, 10, 10),
                    alignment = TextAnchor.LowerLeft
                };

                Notifications.Container = new GUIStyle()
                {
                    padding = new RectOffset(10, 10, 110, 10),
                };

                Notifications.Label = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 15,
                    padding = new RectOffset(5, 5, 5, 5),
                    margin = new RectOffset(10, 10, 10, 10),
                    alignment = TextAnchor.MiddleLeft
                };
            }



            public static GUIStyle VersionLabel;

            public static class Notifications
            {
                public static GUIStyle Container = new GUIStyle();
                public static GUIStyle Label = new GUIStyle(GUI.skin.box);
            }
        }
    }
}
