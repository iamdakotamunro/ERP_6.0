using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ResourcePublic.Model;
using ResourcePublic.Util;
using Telerik.Web.UI;
using Telerik.Web.UI.Widgets;

namespace ERP.UI.Web.Common
{
    public sealed class ResourceFileBrowserContentProvider : FileBrowserContentProvider
    {
        private static string _relRootPath;//绝对根路径

        private string _firstPath;

        private ResourceItem[] _allItems;

        public ResourceFileBrowserContentProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag)
            : base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
            if (_relRootPath == null)
            {
                _relRootPath = Context.Server.MapPath("~/");
            }

            if (ViewPaths != null && ViewPaths.Length > 0)
            {
                for (int i = 0; i < ViewPaths.Length; i++)
                {
                    ViewPaths[i] = GetAbsolutePath(ViewPaths[i]);
                }
            }

            if (UploadPaths != null && UploadPaths.Length > 0)
            {
                for (int i = 0; i < UploadPaths.Length; i++)
                {
                    UploadPaths[i] = GetAbsolutePath(UploadPaths[i]);
                }
            }

            if (DeletePaths != null && DeletePaths.Length > 0)
            {
                for (int i = 0; i < DeletePaths.Length; i++)
                {
                    DeletePaths[i] = GetAbsolutePath(DeletePaths[i]);
                }
            }

            SelectedUrl = GetAbsolutePath(SelectedUrl);

            // client =new ResourceFileServerClient(new Guid("c8c8ba6b-00da-49c7-8d4a-91675dbb65ba"));

        }

        #region Overrides of FileBrowserContentProvider

        public override DirectoryItem ResolveRootDirectoryAsTree(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new DirectoryItem();
            }

            path = MapPath(path).ToLower();

            if (string.IsNullOrEmpty(_firstPath))
            {
                _firstPath = path;

                var dir = ResouceServiceManager.Client.GetDirectories(path, SearchPatterns);
                var files = ResouceServiceManager.Client.GetFiles(path, SearchPatterns);
                ////TODO 11111
                //if (files != null && dir!=null)
                //{
                    _allItems = dir.Union(files).ToArray();
                //}
                //else if (files == null && dir != null)
                //{
                //    _allItems = dir.ToArray();
                //}
                //else if (files != null && dir == null)
                //{
                //    _allItems = files.ToArray();
                //}
            }

            if (path == _firstPath)
            {
                return GetDir(ToUrl(path));
            }
            var item = GetItem(path);
            if (item == null)
                return null;
            return new DirectoryItem
            {
                Name = item.Name,
                FullPath = item.Path,
                Path = item.Path,
                Location = "",
                Directories = item.HaveChildDir ? new DirectoryItem[1] : new DirectoryItem[0],
                Files = new FileItem[0],
                Tag = "",
                Permissions = (PathPermissions)7
            };
        }

        private ResourceItem GetItem(string path)
        {
            var temppath = path.ToLower();
            return _allItems.FirstOrDefault(o => o.Path.ToLower().Equals(temppath));
        }

        private DirectoryItem GetDir(string path)
        {
            const PathPermissions PERMISSIONS = (PathPermissions)7;
            var dir = GetChildDirs();
            var files = GetFiles(PERMISSIONS);

            return new DirectoryItem
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                Path = path,
                Location = "",
                Directories = dir,
                Files = files,
                Tag = "",
                Permissions = PERMISSIONS
            };
        }

        private FileItem[] GetFiles(PathPermissions permissions)
        {
            if (_allItems == null) return new FileItem[0];

            return
                _allItems.Where(o => !o.IsFolder)
                    .Select(
                        item =>
                            new FileItem(item.Name, Path.GetExtension(item.Name), item.Size, "", item.Url, "",
                                permissions))
                    .ToArray();
        }

        private DirectoryItem[] GetChildDirs()
        {
            if (_allItems == null) return new DirectoryItem[0];
            return _allItems.Where(o => o.IsFolder).Select(item => new DirectoryItem(item.Name, "", ToUrl(item.Path), "", (PathPermissions)7, new FileItem[0], new DirectoryItem[0])).ToArray();
        }

        public override DirectoryItem ResolveDirectory(string path)
        {
            return GetDir(path);
        }

        public override bool CanCreateDirectory
        {
            get
            {
                return true;
            }
        }

        public override string GetFileName(string url)
        {
            return Path.GetFileName(RemoveProtocolNameAndServerName(GetAbsolutePath(url)));
        }

        public override string GetPath(string url)
        {
            string virtualPath = RemoveProtocolNameAndServerName(GetAbsolutePath(url));
            try
            {
                return VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.GetDirectory(virtualPath).Replace("\\", "/")));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public override Stream GetFile(string url)
        {
            byte[] data = ResouceServiceManager.Client.GetFile(MapPath(url));

            if (data == null) return null;

            return new MemoryStream(data);
        }

        public override string StoreBitmap(Bitmap bitmap, string url, ImageFormat format)
        {
            var path = MapPath(url);

            ResouceServiceManager.UploadFile(Path.GetDirectoryName(path), Path.GetFileName(path), ImageUtil.ImageToBytes(bitmap, format));

            return url;
        }

        public override string StoreFile(UploadedFile file, string path, string name, params string[] arguments)
        {
            string targetFullPath = Path.Combine(path, name);

            ResouceServiceManager.UploadFile(MapPath(path), name, ImageUtil.StreamToBytes(file.InputStream));

            return targetFullPath;
        }

        public override string DeleteFile(string path)
        {
            return ResouceServiceManager.Client.DeleteFile(MapPath(path)) ?? string.Empty;
        }

        public override string DeleteDirectory(string path)
        {

            return ResouceServiceManager.Client.DeleteDirectory(MapPath(path)) ?? string.Empty;
        }

        public override string CreateDirectory(string path, string name)
        {
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return "InvalidCharactersInPath";
            }

            return ResouceServiceManager.Client.CreateDirectory(MapPath(path), name) ?? string.Empty;
        }

        //public override string RenameDirectory()

        public override string MoveDirectory(string path, string newPath)
        {
            _firstPath = string.Empty;
            return ResouceServiceManager.Client.MoveDirectory(MapPath(path), MapPath(newPath)) ?? string.Empty;
        }

        public override string MoveFile(string path, string newPath)
        {
            return ResouceServiceManager.Client.MoveFile(MapPath(path), MapPath(newPath)) ?? string.Empty;
        }

        public override string CopyDirectory(string path, string newPath)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(newPath))
            {
                return "MessageCannotWriteToFolder";
            }

            return ResouceServiceManager.Client.CopyDirectory(MapPath(path), MapPath(newPath)) ?? string.Empty;
        }

        public override string CopyFile(string path, string newPath)
        {
            return ResouceServiceManager.Client.CopyFile(MapPath(path), MapPath(newPath)) ?? string.Empty;
        }

        #endregion

        internal bool IsValid(FileInfo file)
        {
            return true;
        }

        internal bool IsValid(DirectoryInfo dir)
        {
            return true;
        }

        internal PathPermissions GetPermissions(string path)
        {
            var permissions = PathPermissions.Read;
            if (CanUpload(path))
                permissions |= PathPermissions.Upload;
            if (CanDelete(path))
                permissions |= PathPermissions.Delete;

            return permissions;

        }

        private bool CanUpload(string path)
        {
            return UploadPaths.Any(uploadPath => IsParentOf(uploadPath, path));
        }

        private bool CanDelete(string path)
        {
            return DeletePaths.Any(deletePath => IsParentOf(deletePath, path));
        }

        private static bool IsParentOf(string virtualParent, string virtualChild)
        {
            //original function did not cover all cases - e.g. parent="/test" and child="/testing" should return false
            if (virtualChild.StartsWith(virtualParent, StringComparison.OrdinalIgnoreCase))
            {
                if (virtualParent.Length == virtualChild.Length)
                {
                    return true;
                }
                //if the parent ends with slash, or the child has a slash after the parent part
                //we should be OK
                int len = virtualParent.Length;
                if (virtualChild[len] == '/' || virtualChild[len] == '\\' ||
                    virtualChild[len - 1] == '/' || virtualChild[len - 1] == '\\')
                    return true;
            }
            return false;
        }

        private string GetAbsolutePath(string path)
        {
            path = path.Replace("~/", VirtualPathUtility.AppendTrailingSlash(Context.Request.ApplicationPath));
            if (!string.IsNullOrEmpty(path))
            {// VirtualPathUtility.RemoveTrailingSlash returns null if the string is empty
                path = VirtualPathUtility.RemoveTrailingSlash(path);
            }
            return path;

        }

        internal static string ToUrl(string path)
        {
            return path.Replace("\\", "/");
        }

        internal string MapPath(string path)
        {
            return "\\" + Context.Server.MapPath(path).Replace(_relRootPath, "");
        }
    }
}