using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace SharpIce {
    public class Metadata {
        public static readonly string CoreVersion = Marshal.PtrToStringUTF8(
            Core.ice_metadata_get_version()
        );
        public static readonly string MinCoreVersion = "0.4.0";

        public static void EnsureCoreVersion() {
            System.Version currentCoreVersion = new System.Version(CoreVersion);
            System.Version requiredMinCoreVersion = new System.Version(MinCoreVersion);

            System.Console.WriteLine(
                "Core version: {0}.{1}.{2}",
                currentCoreVersion.Major,
                currentCoreVersion.Minor,
                currentCoreVersion.Build
            );

            if(
                currentCoreVersion.Major != requiredMinCoreVersion.Major
                || currentCoreVersion.Minor < requiredMinCoreVersion.Minor
                || (
                    currentCoreVersion.Major == 0
                    && currentCoreVersion.Minor != requiredMinCoreVersion.Minor
                )
                || (
                    currentCoreVersion.Major == 0
                    && currentCoreVersion.Build < requiredMinCoreVersion.Build
                )
            ) {
                throw new System.Exception(
                    String.Format("Version mismatch: v{0} required", MinCoreVersion)
                );
            }
        }
    }
}
