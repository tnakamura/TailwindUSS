using System;

namespace TailwindUSS.Editor
{
    [Serializable]
    internal sealed class TailwindUssConfig
    {
        public string[] inputGlobs = { "Assets/**/*.uxml" };
        public string outputUssPath = "Assets/Generated/TailwindUSS.generated.uss";
        public bool autoAttachGeneratedUss;

        public static TailwindUssConfig CreateDefault()
        {
            return new TailwindUssConfig();
        }
    }
}
