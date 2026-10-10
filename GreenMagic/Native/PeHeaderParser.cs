using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GreenMagic.Native
{
	public class PeHeaderParser
	{
		public PeHeaderParser(string peFile, Memory memory)
		{
			this.ModulePtr = Imports.LoadLibrary(peFile);
			if (this.ModulePtr == IntPtr.Zero)
			{
				throw new FileNotFoundException();
			}
			this._memory = memory;
			this.ParseHeaders();
			Imports.FreeLibrary(this.ModulePtr);
		}

		public PeHeaderParser(IntPtr hModule, Memory memory)
		{
			if (hModule == IntPtr.Zero)
			{
				throw new FileNotFoundException();
			}
			this._memory = memory;
			this.ModulePtr = hModule;
			this.ParseHeaders();
		}

		public PeHeaderParser.ImageDosHeader DosHeader
		{
			[CompilerGenerated]
			get
			{
				return this._dosHeader;
			}
			[CompilerGenerated]
			private set
			{
				this._dosHeader = value;
			}
		}

		public PeHeaderParser.ImageNtHeader NtHeader
		{
			[CompilerGenerated]
			get
			{
				return this._ntHeader;
			}
			[CompilerGenerated]
			private set
			{
				this._ntHeader = value;
			}
		}

		private void ParseHeaders()
		{
			// Use 64-bit-capable reads so modules loaded above 4 GB
			// (native x64 images) parse correctly as well.
			ulong moduleAddress = (ulong)this.ModulePtr.ToInt64();

			this.DosHeader = this._memory.ReadStruct64<PeHeaderParser.ImageDosHeader>(moduleAddress);
			if (this.DosHeader.e_magic != 23117)
			{
				return;
			}

			ulong ntAddress = moduleAddress + (uint)this.DosHeader.e_lfanew;

			// The optional header magic is the first field of the optional
			// header: NT signature (4) + file header (20) bytes in.
			const ulong OptionalHeaderOffset = 4UL + 20UL;
			ushort optionalMagic = this._memory.ReadStruct64<PeHeaderParser.ImageOptionalHeader64>(ntAddress + OptionalHeaderOffset).Magic;
			this.Is64BitImage = optionalMagic == PeHeaderConstants.IMAGE_NT_OPTIONAL_HDR64_MAGIC;

			if (this.Is64BitImage)
			{
				this.NtHeader64 = this._memory.ReadStruct64<PeHeaderParser.ImageNtHeader64>(ntAddress);
			}
			else
			{
				this.NtHeader = this._memory.ReadStruct64<PeHeaderParser.ImageNtHeader>(ntAddress);
			}
		}

        public IntPtr ModulePtr;

        private readonly Memory _memory;

        // Backing fields for auto-properties (compiler-generated)
        private ImageDosHeader _dosHeader;
        private ImageNtHeader _ntHeader;
        private ImageNtHeader64 _ntHeader64;

        /// <summary>
        /// True when the parsed image is a PE32+ (64-bit) executable.
        /// When true, <see cref="NtHeader64"/> holds the parsed headers
        /// and <see cref="NtHeader"/> remains default-valued; otherwise
        /// the reverse holds.
        /// </summary>
        public bool Is64BitImage
        {
            [CompilerGenerated]
            get;
            [CompilerGenerated]
            private set;
        }

        public ImageNtHeader64 NtHeader64
        {
            [CompilerGenerated]
            get
            {
                return this._ntHeader64;
            }
            [CompilerGenerated]
            private set
            {
                this._ntHeader64 = value;
            }
        }

        public struct ImageDataDirectory
        {
            public override string ToString()
            {
                return string.Format("--START DATA DIRECTORY--\n VirtualAddress: {0}, Size: {1}\n--END DATA DIRECTORY--\n", this.VirtualAddress.ToString("X"), this.Size.ToString("X"));
            }

            public uint VirtualAddress;

            public uint Size;
        }

        public struct ImageDosHeader
        {
            public ushort e_magic;

            public ushort e_cblp;

            public ushort e_cp;

            public ushort e_crlc;

            public ushort e_cparhdr;

            public ushort e_minalloc;

            public ushort e_maxalloc;

            public ushort e_ss;

            public ushort e_sp;

            public ushort e_csum;

            public ushort e_ip;

            public ushort e_cs;

            public ushort e_lfarlc;

            public ushort e_ovno;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] e_res1;

            public ushort e_oemid;

            public ushort e_oeminfo;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public ushort[] e_res2;

            public int e_lfanew;
        }

        public struct ImageFileHeader
        {
            // Note: this type is marked as 'beforefieldinit'.
            static ImageFileHeader()
            {
            }

            public override string ToString()
            {
                string executionResult;
                try
                {
                    string format = "--START FILE HEADER--\n Machine: {0}, NumberOfSections: {1}, TimeDateStamp: {2}, PointerToSymbolTable: {3}, NumberOfSymbols: {4}, SizeOfOptionalHeader: {5}, Characteristics: {6}\n--END FILE HEADER--\n";
                    object[] array = new object[]
                    {
                        this.Machine.ToString("X"),
                        this.NumberOfSections.ToString("X"),
                        this.TimeDateStamp.ToString("X"),
                        this.PointerToSymbolTable.ToString("X"),
                        this.NumberOfSymbols.ToString("X"),
                        this.SizeOfOptionalHeader.ToString("X"),
                        this.Characteristics.ToString("X")
                    };
                    executionResult = string.Format(format, array);
                }
                catch (Exception ex)
                {
                    throw;
                }
                return executionResult;
            }

            public ushort Machine;

            public ushort NumberOfSections;

            public uint TimeDateStamp;

            public uint PointerToSymbolTable;

            public uint NumberOfSymbols;

            public ushort SizeOfOptionalHeader;

            public ushort Characteristics;
        }

        public struct ImageNtHeader
        {
            // Note: this type is marked as 'beforefieldinit'.
            static ImageNtHeader()
            {
            }

            public override string ToString()
            {
                string executionResult;
                try
                {
                    executionResult = string.Format("Signature: {0},\n{1}\n{2}", this.Signature.ToString("X"), this.FileHeader, this.OptionalHeader);
                }
                catch (Exception)
                {
                    throw;
                }
                return executionResult;
            }

            public uint Signature;

            public PeHeaderParser.ImageFileHeader FileHeader;

            public PeHeaderParser.ImageOptionalHeader OptionalHeader;
        }

        /// <summary>
        /// IMAGE_NT_HEADERS64 — the PE32+ variant used by 64-bit images.
        /// Selected by <see cref="ParseHeaders"/> when the optional header
        /// magic is IMAGE_NT_OPTIONAL_HDR64_MAGIC (0x20B).
        /// </summary>
        public struct ImageNtHeader64
        {
            public uint Signature;

            public PeHeaderParser.ImageFileHeader FileHeader;

            public PeHeaderParser.ImageOptionalHeader64 OptionalHeader;
        }

        /// <summary>
        /// IMAGE_OPTIONAL_HEADER64 (PE32+). Differences from the 32-bit
        /// variant: no BaseOfData field, 8-byte ImageBase and four
        /// Reserve/Commit sizes, and the magic is 0x20B instead of 0x10B.
        /// </summary>
        public struct ImageOptionalHeader64
        {
            public ushort Magic;

            public byte MajorLinkerVersion;

            public byte MinorLinkerVersion;

            public uint SizeOfCode;

            public uint SizeOfInitializedData;

            public uint SizeOfUninitializedData;

            public uint AddressOfEntryPoint;

            public uint BaseOfCode;

            // NOTE: PE32+ has no BaseOfData member.

            public ulong ImageBase;

            public uint SectionAlignment;

            public uint FileAlignment;

            public ushort MajorOperatingSystemVersion;

            public ushort MinorOperatingSystemVersion;

            public ushort MajorImageVersion;

            public ushort MinorImageVersion;

            public ushort MajorSubsystemVersion;

            public ushort MinorSubsystemVersion;

            public uint Win32VersionValue;

            public uint SizeOfImage;

            public uint SizeOfHeaders;

            public uint CheckSum;

            public ushort Subsystem;

            public ushort DllCharacteristics;

            public ulong SizeOfStackReserve;

            public ulong SizeOfStackCommit;

            public ulong SizeOfHeapReserve;

            public ulong SizeOfHeapCommit;

            public uint LoaderFlags;

            public uint NumberOfRvaAndSizes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public PeHeaderParser.ImageDataDirectory[] DataDirectory;
        }

        public struct ImageOptionalHeader
        {
            // Note: this type is marked as 'beforefieldinit'.
            static ImageOptionalHeader()
            {
            }

            public override string ToString()
            {
                string executionResult;
                try
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (PeHeaderParser.ImageDataDirectory imageDataDirectory in this.DataDirectory)
                    {
                        stringBuilder.Append(imageDataDirectory.ToString());
                    }
                    string format = "-- START OPTIONAL HEADER --\n Magic: {0}, MajorLinkerVersion: {1}, MinorLinkerVersion: {2}, SizeOfCode: {3}, SizeOfInitializedData: {4}, SizeOfUninitializedData: {5}, AddressOfEntryPoint: {6}, BaseOfCode: {7}, BaseOfData: {8}, ImageBase: {9}, SectionAlignment: {10}, FileAlignment: {11}, MajorOperatingSystemVersion: {12}, MinorOperatingSystemVersion: {13}, MajorImageVersion: {14}, MinorImageVersion: {15}, MajorSubsystemVersion: {16}, MinorSubsystemVersion: {17}, Win32VersionValue: {18}, SizeOfImage: {19}, SizeOfHeaders: {20}, CheckSum: {21}, Subsystem: {22}, DllCharacteristics: {23}, SizeOfStackReserve: {24}, SizeOfStackCommit: {25}, SizeOfHeapReserve: {26}, SizeOfHeapCommit: {27}, LoaderFlags: {28}, NumberOfRvaAndSizes: {29}, DataDirectory: {30}\n--END OPTIONAL HEADER--\n";
                    object[] array = new object[]
                    {
                        this.Magic.ToString("X"),
                        this.MajorLinkerVersion.ToString("X"),
                        this.MinorLinkerVersion.ToString("X"),
                        this.SizeOfCode.ToString("X"),
                        this.SizeOfInitializedData.ToString("X"),
                        this.SizeOfUninitializedData.ToString("X"),
                        this.AddressOfEntryPoint.ToString("X"),
                        this.BaseOfCode.ToString("X"),
                        this.BaseOfData.ToString("X"),
                        this.ImageBase.ToString("X"),
                        this.SectionAlignment.ToString("X"),
                        this.FileAlignment.ToString("X"),
                        this.MajorOperatingSystemVersion.ToString("X"),
                        this.MinorOperatingSystemVersion.ToString("X"),
                        this.MajorImageVersion.ToString("X"),
                        this.MinorImageVersion.ToString("X"),
                        this.MajorSubsystemVersion.ToString("X"),
                        this.MinorSubsystemVersion.ToString("X"),
                        this.Win32VersionValue.ToString("X"),
                        this.SizeOfImage.ToString("X"),
                        this.SizeOfHeaders.ToString("X"),
                        this.CheckSum.ToString("X"),
                        this.Subsystem.ToString("X"),
                        this.DllCharacteristics.ToString("X"),
                        this.SizeOfStackReserve.ToString("X"),
                        this.SizeOfStackCommit.ToString("X"),
                        this.SizeOfHeapReserve.ToString("X"),
                        this.SizeOfHeapCommit.ToString("X"),
                        this.LoaderFlags.ToString("X"),
                        this.NumberOfRvaAndSizes.ToString("X"),
                        stringBuilder
                    };
                    executionResult = string.Format(format, array);
                }
                catch (Exception)
                {
                    throw;
                }
                return executionResult;
            }

            public ushort Magic;

            public byte MajorLinkerVersion;

            public byte MinorLinkerVersion;

            public uint SizeOfCode;

            public uint SizeOfInitializedData;

            public uint SizeOfUninitializedData;

            public uint AddressOfEntryPoint;

            public uint BaseOfCode;

            public uint BaseOfData;

            public uint ImageBase;

            public uint SectionAlignment;

            public uint FileAlignment;

            public ushort MajorOperatingSystemVersion;

            public ushort MinorOperatingSystemVersion;

            public ushort MajorImageVersion;

            public ushort MinorImageVersion;

            public ushort MajorSubsystemVersion;

            public ushort MinorSubsystemVersion;

            public uint Win32VersionValue;

            public uint SizeOfImage;

            public uint SizeOfHeaders;

            public uint CheckSum;

            public ushort Subsystem;

            public ushort DllCharacteristics;

            public uint SizeOfStackReserve;

            public uint SizeOfStackCommit;

            public uint SizeOfHeapReserve;

            public uint SizeOfHeapCommit;

            public uint LoaderFlags;

            public uint NumberOfRvaAndSizes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public PeHeaderParser.ImageDataDirectory[] DataDirectory;
        }

        public class PeHeaderConstants
        {
            public PeHeaderConstants()
            {
                try
                {
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public const int IMAGE_DOS_SIGNATURE = 23117;

            public const int IMAGE_FILE_32BIT_MACHINE = 256;

            public const int IMAGE_FILE_AGGRESIVE_WS_TRIM = 16;

            public const int IMAGE_FILE_BYTES_REVERSED_HI = 32768;

            public const int IMAGE_FILE_BYTES_REVERSED_LO = 128;

            public const int IMAGE_FILE_DEBUG_STRIPPED = 512;

            public const int IMAGE_FILE_DLL = 8192;

            public const int IMAGE_FILE_EXECUTABLE_IMAGE = 2;

            public const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 32;

            public const int IMAGE_FILE_LINE_NUMS_STRIPPED = 4;

            public const int IMAGE_FILE_LOCAL_SYMS_STRIPPED = 8;

            public const int IMAGE_FILE_MACHINE_ALPHA = 388;

            public const int IMAGE_FILE_MACHINE_ALPHA64 = 644;

            public const int IMAGE_FILE_MACHINE_AM33 = 467;

            public const int IMAGE_FILE_MACHINE_AMD64 = 34404;

            public const int IMAGE_FILE_MACHINE_ARM = 448;

            public const int IMAGE_FILE_MACHINE_CEE = 49390;

            public const int IMAGE_FILE_MACHINE_CEF = 3311;

            public const int IMAGE_FILE_MACHINE_EBC = 3772;

            public const int IMAGE_FILE_MACHINE_I386 = 332;

            public const int IMAGE_FILE_MACHINE_IA64 = 512;

            public const int IMAGE_FILE_MACHINE_M32R = 36929;

            public const int IMAGE_FILE_MACHINE_MIPS16 = 614;

            public const int IMAGE_FILE_MACHINE_MIPSFPU = 870;

            public const int IMAGE_FILE_MACHINE_MIPSFPU16 = 1126;

            public const int IMAGE_FILE_MACHINE_POWERPC = 496;

            public const int IMAGE_FILE_MACHINE_POWERPCFP = 497;

            public const int IMAGE_FILE_MACHINE_R10000 = 360;

            public const int IMAGE_FILE_MACHINE_R3000 = 354;

            public const int IMAGE_FILE_MACHINE_R4000 = 358;

            public const int IMAGE_FILE_MACHINE_SH3 = 418;

            public const int IMAGE_FILE_MACHINE_SH3DSP = 419;

            public const int IMAGE_FILE_MACHINE_SH3E = 420;

            public const int IMAGE_FILE_MACHINE_SH4 = 422;

            public const int IMAGE_FILE_MACHINE_SH5 = 424;

            public const int IMAGE_FILE_MACHINE_THUMB = 450;

            public const int IMAGE_FILE_MACHINE_TRICORE = 1312;

            public const int IMAGE_FILE_MACHINE_UNKNOWN = 0;

            public const int IMAGE_FILE_MACHINE_WCEMIPSV2 = 361;

            public const int IMAGE_FILE_NET_RUN_FROM_SWAP = 2048;

            public const int IMAGE_FILE_RELOCS_STRIPPED = 1;

            public const int IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 1024;

            public const int IMAGE_FILE_SYSTEM = 4096;

            public const int IMAGE_FILE_UP_SYSTEM_ONLY = 16384;

            public const int IMAGE_NT_OPTIONAL_HDR32_MAGIC = 267;

            public const int IMAGE_NT_OPTIONAL_HDR64_MAGIC = 523;

            public const int IMAGE_NT_SIGNATURE = 1346699264;

            public const int IMAGE_OS2_SIGNATURE = 20037;

            public const int IMAGE_OS2_SIGNATURE_LE = 19525;

            public const int IMAGE_ROM_OPTIONAL_HDR_MAGIC = 263;

            public const int IMAGE_SIZEOF_FILE_HEADER = 20;
        }
    }
}
