using System.Reflection;
using System.Runtime.InteropServices;

namespace BCDecNet;

internal static partial class NativeMethods {
	private const string LibName = "bcdec";

	static NativeMethods() {
		NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
	}

	private static nint DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
		if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out var handle)) {
			return handle;
		}

		if (searchPath != null && !searchPath.Value.HasFlag(DllImportSearchPath.AssemblyDirectory)) {
			return nint.Zero;
		}

		var name = Path.GetFileNameWithoutExtension(libraryName);
		var cwd = AppDomain.CurrentDomain.BaseDirectory;

		string ext;
		if (OperatingSystem.IsWindows()) {
			ext = ".dll";
		} else if (OperatingSystem.IsLinux()) {
			ext = ".so";
		} else if (OperatingSystem.IsMacOS()) {
			ext = ".dylib";
		} else {
			return nint.Zero;
		}

		foreach (var dir in new[] { Path.Combine(cwd, $"runtimes/{RuntimeInformation.RuntimeIdentifier}/native/"), cwd }) {
			foreach (var libName in new[] { name, "lib" + name, name + "-0", $"lib{name}-0" }) {
				var target = Path.Combine(dir, libName) + ext;
				if (File.Exists(target)) {
					var ptr = NativeLibrary.Load(target);
					if (ptr != nint.Zero) {
						return ptr;
					}
				}
			}
		}

		return nint.Zero;
	}

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc1(nint compressedBlock, nint decompressedBlock, int destinationPitch);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc2(nint compressedBlock, nint decompressedBlock, int destinationPitch);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc3(nint compressedBlock, nint decompressedBlock, int destinationPitch);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc4(nint compressedBlock, nint decompressedBlock, int destinationPitch, [MarshalAs(UnmanagedType.I4)] bool isSigned);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc5(nint compressedBlock, nint decompressedBlock, int destinationPitch, [MarshalAs(UnmanagedType.I4)] bool isSigned);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc4_float(nint compressedBlock, nint decompressedBlock, int destinationPitch, [MarshalAs(UnmanagedType.I4)] bool isSigned);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc5_float(nint compressedBlock, nint decompressedBlock, int destinationPitch, [MarshalAs(UnmanagedType.I4)] bool isSigned);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc6h_float(nint compressedBlock, nint decompressedBlock, int destinationPitch, [MarshalAs(UnmanagedType.I4)] bool isSigned);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc6h_half(nint compressedBlock, nint decompressedBlock, int destinationPitch, [MarshalAs(UnmanagedType.I4)] bool isSigned);

	[LibraryImport(LibName), DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
	internal static partial nint bcdec_bc7(nint compressedBlock, nint decompressedBlock, int destinationPitch);
}
