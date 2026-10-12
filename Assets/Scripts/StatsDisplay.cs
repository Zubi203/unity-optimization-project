using UnityEngine;
using Unity.Profiling;
using UnityEngine.Profiling;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayLabel;

    private float deltaTime = 0.0f;
    private float updateRate = 0.1f;
    private float updateTimer = 0.0f;

    private int entityCount = 0;
    private string entityText;

    private float millisecondsTimePerFrame;
    private float framesPerSecond;
    private string fpsText;

    private long totalMemoryMB;
    private long allocatedMemoryMB;
    private long gcMemoryMB;
    private string gcText;

    private ProfilerRecorder drawCallsRecorder;
    private ProfilerRecorder batchesRecorder;
    private long drawCalls;
    private long batches;
    private string drawCallText;

    private string displayText;

    EntityTracker entityTracker
    {
        get
        {
            if (ServiceLocator.Instance.TryGetService(typeof(EntityTracker)))
            {
                return ServiceLocator.Instance.GetService(typeof(EntityTracker)).GetComponent<EntityTracker>();
            }

            return null;
        }
    }

    void Start()
    {
        Application.targetFrameRate = -1;
        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        batchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
    }

    void OnDestroy()
    {
        drawCallsRecorder.Dispose();
        batchesRecorder.Dispose();
    }

    void Update()
    {
        deltaTime = (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (updateTimer > 0.0f)
        {
            updateTimer -= Time.deltaTime;
            return;
        }

        updateTimer = updateRate;
        //if (displayText == null){return;}

        entityText = string.Format("Active Entities: {0}", entityTracker.GetEntityCount());

        millisecondsTimePerFrame = deltaTime * 1000.0f;
        framesPerSecond = 1.0f / deltaTime;
        fpsText = string.Format("FPS: {0:0} ({1:0.0} ms)", framesPerSecond, millisecondsTimePerFrame);


        totalMemoryMB = Profiler.GetTotalReservedMemoryLong() / (1024 * 1024);
        allocatedMemoryMB = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
        gcMemoryMB = System.GC.GetTotalMemory(false) / (1024 * 1024);
        gcText = string.Format("GC Allocation: {0} MB | Allocated: {1} MB | Reserved: {2} MB", gcMemoryMB, allocatedMemoryMB, totalMemoryMB);

        drawCalls = drawCallsRecorder.CurrentValue;
        batches = batchesRecorder.CurrentValue;
        drawCallText = string.Format("Draw Calls: {0} | Batches: {1}", drawCalls, batches);

        displayText = string.Format("<b>Stats:</b>\n\n{0}\n\n{1}\n\n{2}\n\n{3}", entityText, fpsText, gcText, drawCallText);
        displayLabel.text = displayText;
    }
}
