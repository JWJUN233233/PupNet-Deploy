// -----------------------------------------------------------------------------
// PROJECT   : PupNet
// COPYRIGHT : Andy Thomas (C) 2022-23
// LICENSE   : GPL-3.0-or-later
// HOMEPAGE  : https://github.com/kuiperzone/PupNet
//
// PupNet is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later version.
//
// PupNet is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with PupNet. If not, see <https://www.gnu.org/licenses/>.
// -----------------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

namespace KuiperZone.PupNet;

/// <summary>
/// Wraps standard file operations with desired behavior and console output.
/// It's mainly for aesthetic information purposes.
/// </summary>
public class FileOps
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public FileOps(string? root = null)
    {
        Root = root;
    }

    /// <summary>
    /// Gets the root directory for operations. This is used for display purposes only where directory path is
    /// removed from the displayed path.
    /// </summary>
    public string? Root { get; }

    /// <summary>
    /// Gets or sets the displays of path information. Default is true.
    /// </summary>
    public bool DisplayPath { get; set; } = true;

    /// <summary>
    /// Asserts file exist. Does nothing if file is null.
    /// </summary>
    public void AssertExists(string? filepath)
    {
        if (filepath != null)
        {
            Write("Exists?: ", filepath);

            if (!File.Exists(filepath))
            {
                WriteLine(" ... FAILED");
                throw new FileNotFoundException("File not found " + filepath);
            }

            WriteLine(" ... OK");
        }
    }

    /// <summary>
    /// Ensures directory exists. Does nothing if dir is null.
    /// </summary>
    public void CreateDirectory(string? dir)
    {
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            try
            {
                Write("Create Directory: ", dir);
                Directory.CreateDirectory(dir);
                WriteLine(" ... OK");
            }
            catch
            {
                WriteLine(" ... FAILED");
                throw;
            }
        }
    }

    /// <summary>
    /// Ensures directory is deleted (recursive). Does nothing if dir is null.
    /// </summary>
    public void RemoveDirectory(string? dir)
    {
        if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
        {
            try
            {
                Write("Remove: ", dir);
                Directory.Delete(dir, true);
                WriteLine(" ... OK");
            }
            catch
            {
                WriteLine(" ... FAILED");
                throw;
            }
        }
    }


    /// <summary>
    /// Copies directory. Does not create destination. Does nothing if either value is null.
    /// </summary>
    public void CopyDirectory(string? src, string? dst)
    {
        if (!string.IsNullOrEmpty(src) && !string.IsNullOrEmpty(dst))
        {
            try
            {
                Write("Populate: ", dst);

                if (!Directory.Exists(dst))
                {
                    throw new DirectoryNotFoundException("Directory not found " + dst);
                }

                FileSystem.CopyDirectory(src, dst);
                WriteLine(" ... OK");
            }
            catch
            {
                WriteLine(" ... FAILED");
                throw;
            }
        }

    }

    /// <summary>
    /// Copies single single file. Does nothing if either value is null.
    /// </summary>
    public void CopyFile(string? src, string? dst, bool ensureDirectory = false)
    {
        if (!string.IsNullOrEmpty(src) && !string.IsNullOrEmpty(dst))
        {
            if (ensureDirectory)
            {
                CreateDirectory(Path.GetDirectoryName(dst));
            }

            try
            {
                Write("Create File: ", dst);
                File.Copy(src, dst);
                WriteLine(" ... OK");
            }
            catch
            {
                WriteLine(" ... FAILED");
                throw;
            }
        }
    }

    /// <summary>
    /// Writes file content. Does nothing if either value is null.
    /// </summary>
    public void WriteFile(string? path, string? content)
    {
        if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(content))
        {
            try
            {
                Write("Create File: ", path);
                File.WriteAllText(path, content);
                WriteLine(" ... OK");
            }
            catch
            {
                WriteLine(" ... FAILED");
                throw;
            }
        }
    }

    /// <summary>
    /// Runs the command.
    /// </summary>
    public void Exec(string cmd)
    {
        WriteLine(cmd);
        ExecInternal(cmd);
    }

    private void ExecInternal(string cmd)
    {
        string? args = null;
        int idx = cmd.IndexOf(' ');

        if (idx > 0)
        {
            args = cmd.Substring(idx + 1).Trim();
            cmd = cmd.Substring(0, idx).Trim();
        }

        var info = new ProcessStartInfo
        {
            Arguments = args,
            CreateNoWindow = true,
            FileName = cmd,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
        };

        var proc = Process.Start(info) ??
            throw new InvalidOperationException($"{cmd} failed");

        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            throw new InvalidOperationException($"Command {cmd} returned non-zero exit code");
        }
    }

    private void Write(string prefix, string? path = null)
    {
        if (DisplayPath)
        {
            Console.Write(prefix);

            if (path != null && Root != null)
            {
                path = Path.GetRelativePath(Root, path);
            }

            Console.Write(path);
        }
    }

    private void WriteLine(string prefix, string? path = null)
    {
        if (DisplayPath)
        {
            Write(prefix, path);
            Console.WriteLine();
        }
    }

}
