/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Text.RegularExpressions;

namespace Corsinvest.AppHero.Core.Service;

public class FileStorageService : IFileStorageService
{
    public async Task<string> UploadAsync(Stream stream, string fileName, string subFolder)
    {
        var retPath = string.Empty;

        stream.Position = 0;
        if (stream.Length > 0)
        {
            var folderName = Path.Combine("files", subFolder);
            var pathToSave = Path.Combine(ApplicationHelper.PathData, folderName);

            if (!Directory.Exists(pathToSave)) { Directory.CreateDirectory(pathToSave); }

            var fileNameDest = fileName.Trim('"');
            fileNameDest = fileNameDest.Replace(" ", "-");
            fileNameDest = Regex.Replace(fileNameDest, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);

            var fullPath = Path.Combine(pathToSave, fileNameDest);
            retPath = Path.Combine(folderName, fileNameDest);

            if (File.Exists(retPath))
            {
                retPath = NextAvailableFilename(retPath);
                fullPath = NextAvailableFilename(fullPath);
            }

            using var fs = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fs);
        }

        return retPath.Replace(@"\", "/");
    }

    public void Remove(string filename, string subFolder)
    {
        var path = Path.Combine("files", subFolder, filename);
        if (File.Exists(path)) { File.Delete(path); }
    }

    private static string NextAvailableFilename(string path)
    {
        const string numberPattern = " ({0})";

        if (!File.Exists(path))
        {
            // Short-cut if already available
            return path;
        }
        else if (Path.HasExtension(path))
        {
            // If path has extension then insert the number pattern just before the extension and return next filename
            return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));
        }
        else
        {
            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }
    }

    private static string GetNextFilename(string pattern)
    {
        var index = 0;
        string? filename;
        do
        {
            index++;
            filename = string.Format(pattern, 1);
        } while (File.Exists(filename));

        return filename;
    }
}
