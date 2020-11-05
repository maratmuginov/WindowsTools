using FileCleanup.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCleanup.Helpers
{
    public static class Utils
    {
        // TODO : This is too much code, would be easier to maintain if we deserialized it from a file.
        private static readonly Dictionary<FileType, string[]> FileTypeExtensions = new Dictionary<FileType, string[]>
        {
            {FileType.picture, new[]
            {
                ".jpg", 
                ".jpeg", 
                ".img", 
                ".png", 
                ".gif", 
                ".ai", 
                ".bmp", 
                ".ico", 
                ".ps", 
                ".psd", 
                "svg", 
                ".tiff", 
                ".tif"
            }},
            {FileType.movie, new []{
                ".3g2",
                ".3gp",
                ".avi",
                ".flv",
                ".h264",
                ".m4v",
                ".mkv",
                ".mov",
                ".mp4",
                ".mpg",
                ".mpeg",
                ".rm",
                ".swf",
                ".vob",
                ".wmv"
            }},
            {FileType.picture, new []{
                ".mp3",
                ".aif",
                ".cda",
                ".mid",
                ".midi",
                ".mpa",
                ".ogg",
                ".wav",
                ".wma",
                ".wpl"
            }},
            {FileType.document, new[]{
                ".doc",
                ".docx",
                ".odt",
                ".pdf",
                ".rtf",
                ".tex",
                ".txt",
                ".wpd"
            }},
            {FileType.log, new []{
                ".log"
            }},
            {FileType.compressed, new []{
                ".7z",
                ".arj",
                ".deb",
                ".pkg",
                ".rar",
                ".rpm",
                ".tar.gz",
                ".z",
                ".zip"
            }},
            {FileType.media, new []{
                ".bin",
                ".dmg",
                ".iso",
                ".toast",
                ".vcd"
            }},
            {FileType.data, new []{
                ".csv",
                ".dat",
                ".db",
                ".dbf",
                ".mdb",
                ".sav",
                ".sql",
                ".tar",
                ".xml"
            }},
            {FileType.email, new []{
                ".email",
                ".eml",
                ".emlx",
                ".msg",
                ".oft",
                ".ost",
                ".pst",
                ".vcf"
            }},
            {FileType.executable, new []{
                ".apk",
                ".bat",
                ".bin",
                ".cgi",
                ".pl",
                ".com",
                ".exe",
                ".gadget",
                ".jar",
                ".msi",
                ".py",
                ".wsf"
            }},
            {FileType.font, new[]{
                ".fnt",
                ".fon",
                ".otf",
                ".ttf"
            }},
            {FileType.internet, new[]{
                ".asp",
                ".aspx",
                ".cer",
                ".cfm",
                ".cgi",
                ".pl",
                ".css",
                ".htm",
                ".html",
                ".js",
                ".jsp",
                ".part",
                ".php",
                ".py",
                ".rss",
                ".xhtml"
            }},
            {FileType.presentation, new []{
                ".key",
                ".odp",
                ".pps",
                ".ppt",
                ".pptx"
            }},
            {FileType.code, new[]{
                ".c",
                ".cgi",
                ".pl",
                ".class",
                ".cpp",
                ".cs",
                ".h",
                ".java",
                ".php",
                ".py",
                ".sh",
                ".swift",
                ".vb"
            }},
            {FileType.spreadsheet, new []
            {
                ".ods",
                ".xls",
                ".xlsm",
                ".xlsx"
            }},
            {FileType.system, new []
            {
                ".bak",
                ".cab",
                ".cfg",
                ".cpl",
                ".cur",
                ".dll",
                ".dmp",
                ".drv",
                ".icns",
                ".ico",
                ".ini",
                ".lnk",
                ".msi",
                ".sys",
                ".tmp"
            }}
        };

        public static FileType GetFileTypeFromExtension(string extension) => 
            FileTypeExtensions.FirstOrDefault(pair => pair.Value.Contains(extension)).Key;

        public static long ConvertSizeToByte(long size, FileSizeType type) => size * type switch
        {
            FileSizeType.Kb => 1024,
            FileSizeType.Mb => (long) Math.Pow(1024, 2),
            FileSizeType.Gb => (long) Math.Pow(1024, 3),
            _ => (long) Math.Pow(1024, 4)
        };

        public static FileSizeType ConvertStringToSizeType(string sizeType)
        {
            Enum.TryParse(sizeType, out FileSizeType result);
            return result;
        }
    }
}
