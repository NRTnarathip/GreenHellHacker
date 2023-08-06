using AIs;
using HackEngine;
using UnityEngine;
namespace GreenHellModHack
{
    public class GreenHellEngineHack
    {
        public void Launch()
        {
            Application.runInBackground = true;

        }
        public void OnUpdate()
        {
            
        }
        public void OnLateUpdate()
        {
        }
        public void OnGUI()
        {
            var AIManagerInstance = AIManager.Get();
            if (AIManagerInstance == null) return;

            var player = Player.Get();
            player.SetSpeedMul(10);
            foreach (AI ai in AIManagerInstance.m_ActiveAIs)
            {
                var transform = ai.transform;
                var name = transform.name;
                var pos = transform.position;
                var boxSize = ai.m_BoxCollider.size;
                Render.DrawESPBox(pos + boxSize / 2, boxSize * 1.2f, new Color(0, 1, 0, .3f), 4f);
                Render.DrawString(pos, $"AI: {name}", 20, Color.white);
            }
        }

    }

}
