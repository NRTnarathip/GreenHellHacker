using MelonLoader;

namespace GreenHellModHack
{
    public class MyMod : MelonMod
    {
        GreenHellEngineHack engine;
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("OnApplicationStart");
        }

        public override void OnLateInitializeMelon() // Runs after OnApplicationStart.
        {
            MelonLogger.Msg("OnApplicationLateStart");
            engine = new GreenHellEngineHack();
            engine.Launch();
        }

        public override void OnUpdate() // Runs once per frame.
        {
            engine.OnUpdate();
        }

        public override void OnLateUpdate() // Runs once per frame after OnUpdate and OnFixedUpdate have finished.
        {
            engine.OnLateUpdate();
        }


        public override void OnGUI() // Can run multiple times per frame. Mostly used for Unity's IMGUI.
        {
            engine.OnGUI();
        }
    }

}
