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
    const float frameTarget = 20f;
    const float deltaFrameTarget = 1f / frameTarget;
    const int deltaMillisecond = (int)(deltaFrameTarget * 1000);


    public const string GameProcessName = "GH";
    ProcessHack gameHack;
    public const string MonoDllName = "mono-2.0-bdwgc.dll";
    IntPtr m_playerPtr = IntPtr.Zero;
    IntPtr m_rootDomainPtr = IntPtr.Zero;
    const IntPtr signatureOffsetToPlayer = -0x7C;
    const IntPtr m_speedMulOffset = 0x484;
    float m_speedMul = 1f;

    bool m_activeRunGameHack = true;
    bool m_activeGUI = true;
    public void LaunchApp()
    {
        m_activeGUI = false;
        gameHack = new ProcessHack(GameProcessName);
        if (!gameHack.Setup() && m_activeRunGameHack)
        {
            return;
        }

        Vector2Int screenSize = new(960, 540);
        if (m_activeGUI)
        {
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
        }
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
        m_activeRunGameHack = false;

        var mono2bdwgcDLL = gameHack.getModule(MonoDllName);
        if (mono2bdwgcDLL == null) return;

        m_rootDomainPtr = Memory.readIntPtr(gameHack.proc, mono2bdwgcDLL.BaseAddress + 0x7BF358);
        Console.WriteLine($"Root Domain Ptr: {m_rootDomainPtr:x}");
        m_playerPtr = Memory.readIntPtr(gameHack.proc, m_rootDomainPtr + 0x5FD0);
        Console.WriteLine($"player ptr: {m_playerPtr:x}");

        //dissect struct player 



    }
    void beforeUpdate()
    {

    }

    void update()
    {
        updateImGUI();
        updateGameHack();
    }
    void updateImGUI()
    {
        if (!m_activeGUI) return;

        var snapshot = window.PumpEvents();
        imguiRenderer.Update(deltaFrameTarget, snapshot);
        ImGui.SetWindowPos(new(0, 0));

        ImGui.Text("Player Movement");
        ImGui.SliderFloat("Speed", ref m_speedMul, 1f, 100f);
    }
    void updateGameHack()
    {
        if (!m_activeRunGameHack) return;

        //scan base player 
        if (m_playerPtr == IntPtr.Zero)
        {
            Console.WriteLine($"Player Pointer Is Valid {m_playerPtr:X}");
            return;
        }
        gameHack.writeFloat(m_playerPtr + m_speedMulOffset, m_speedMul);
    }
    void render()
    {
        if (!m_activeGUI) return;

        // Process ImGui UI
        cmdList.Begin();
        cmdList.SetFramebuffer(graphicsDevice.MainSwapchain.Framebuffer);
        cmdList.ClearColorTarget(0, new RgbaFloat(0.1f, 0.1f, 0.15f, 1f));
        imguiRenderer.Render(graphicsDevice, cmdList);
        cmdList.End();
        graphicsDevice.SubmitCommands(cmdList);
        graphicsDevice.SwapBuffers(graphicsDevice.MainSwapchain);
    }
    void tryExit()
    {
        m_running = true;
    }
}
