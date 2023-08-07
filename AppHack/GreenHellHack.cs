using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
public class GreenHellHack
{
    ImGuiRenderer imguiRenderer;
    GraphicsDevice graphicsDevice;
    CommandList cmdList;
    private Sdl2Window window;

    private bool m_running;
    const float frameTarget = 144f;
    const float deltaFrameTarget = 1f / frameTarget;
    const int deltaMillisecond = (int)(deltaFrameTarget * 1000);


    public const string GameProcessName = "GH";
    ProcessHack gameHack;
    // find base player with nearest signature and offset -7C
    // player = nearest_sig + (-7C)
    public const string MonoDllFileName = "mono-2.0-bdwgc.dll";
    const string signature = "33 33 B3 3E ?? 00 00 00 00 00 00 00 00 00 00 00 FF FF 7F FF";
    string[] signatureAOBs = signature.Split(' ');
    IntPtr m_playerPtr = IntPtr.Zero;
    IntPtr m_rootDomainPtr = IntPtr.Zero;
    const IntPtr signatureOffsetToPlayer = -0x7C;
    const IntPtr m_speedMulOffset = 0x484;
    float m_speedMul = 1f;

    bool m_activeRunGameHack = true;
    bool m_activeGUI = true;
    public void LaunchApp()
    {
        gameHack = new ProcessHack(GameProcessName);
        if (!gameHack.Setup() && m_activeRunGameHack)
        {
            return;
        }
        Vector2Int screenSize = new(960, 540);
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(screenSize.x / 2, screenSize.y / 2, screenSize.x, screenSize.y,
                WindowState.Normal,
                "GreeHell Hack"),
            out window,
            out graphicsDevice);

        imguiRenderer = new ImGuiRenderer(
                    graphicsDevice, graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                    (int)graphicsDevice.MainSwapchain.Framebuffer.Width,
                    (int)graphicsDevice.MainSwapchain.Framebuffer.Height);
        cmdList = graphicsDevice.ResourceFactory.CreateCommandList();
        window.Resized += () =>
        {
            imguiRenderer.WindowResized(window.Width, window.Height);
            graphicsDevice.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
        };
        m_running = true;

        init();
        while (m_running)
        {
            beforeUpdate();
            update();
            render();

            Thread.Sleep(deltaMillisecond);
        }

    }

    void init()
    {
        m_activeGUI = false;
        m_activeRunGameHack = false;

        var mono2bdwgcDLL = gameHack.getModule(MonoDllFileName);
        if (mono2bdwgcDLL == null) return;

        m_rootDomainPtr = Memory.readIntPtr(gameHack.proc,
            mono2bdwgcDLL.BaseAddress + 0x7BF358);
        Console.WriteLine($"Root Domain Ptr: {m_rootDomainPtr:x}");
        //Console.WriteLine("Root Domain Result Call func ptr: " + (Marshal.Get).hex());
        m_playerPtr = Memory.readIntPtr(gameHack.proc,
            m_rootDomainPtr + 0x5FD0);
    }
    void beforeUpdate()
    {

    }

    void update()
    {
        //Update GUI
        if (m_activeGUI)
            updateImGUI();

        //update game hack
        if (m_activeRunGameHack)
            updateGameHack();
    }
    void updateImGUI()
    {
        var snapshot = window.PumpEvents();
        imguiRenderer.Update(deltaFrameTarget, snapshot);
        ImGui.SetWindowPos(new(0, 0));

        ImGui.Text("Player Movement");
        ImGui.SliderFloat("Speed", ref m_speedMul, 1f, 100f);
    }
    void updateGameHack()
    {
        //scan base player 
        if (m_playerPtr == IntPtr.Zero)
        {
            //Console.WriteLine($"Player Pointer Is Valid {m_playerPtr:X}");
            return;
        }
        gameHack.writeFloat(m_playerPtr + m_speedMulOffset, m_speedMul);
    }
    void render()
    {

        // Process ImGui UI
        cmdList.Begin();
        cmdList.SetFramebuffer(graphicsDevice.MainSwapchain.Framebuffer);
        cmdList.ClearColorTarget(0, new RgbaFloat(0.1f, 0.1f, 0.15f, 1f));
        if (m_activeGUI)
        {
            imguiRenderer.Render(graphicsDevice, cmdList);
        }
        cmdList.End();
        graphicsDevice.SubmitCommands(cmdList);
        graphicsDevice.SwapBuffers(graphicsDevice.MainSwapchain);
    }
    void tryExit()
    {
        m_running = true;
    }
}
