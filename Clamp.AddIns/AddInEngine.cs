using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.AddIns
{
    /// <summary>
    /// 插件引擎类
    /// </summary>
    internal abstract class AddInEngine : IAddInEngine
    {
        private List<string> addInFiles = new List<string>();
        private List<string> disabledAddIns = new List<string>();
        private AddInTreeImpl addInTree;

        /// <summary>
        /// 禁止的插件文件
        /// </summary>
        public List<string> DisableAddIns
        {
            get { return disabledAddIns; }
        }

        /// <summary>
        /// 插件文件集合
        /// </summary>
        public List<string> AddInFiles
        {
            get { return this.addInFiles; }
        }

        /// <summary>
        /// 插件树
        /// </summary>
        public AddInTreeImpl AddInTree
        {
            get { return addInTree; }
        }

        public AddInEngine()
        {
            this.addInTree = new AddInTreeImpl();
        }

        /// <summary>
        /// 搜找当前所有的插件文件
        /// </summary>
        /// <param name="addInDir"></param>
        public virtual void AddAddInsFromDirectory(string addInDir)
        {
            if (addInDir == null)
                throw new ArgumentNullException("addInDir");

            addInFiles.AddRange(Directory.GetFiles(addInDir, "AddIn.Xml", SearchOption.AllDirectories));
        }

        /// <summary>
        /// 增加插件文件
        /// </summary>
        /// <param name="addInFile"></param>
        public virtual void AddAddInFile(string addInFile)
        {
            if (addInFile == null)
                throw new ArgumentNullException("addInFile");

            addInFiles.Add(addInFile);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// 激活插件
        /// </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object BuildItem(string path, object parameter)
        {
            return BuildItem(path, parameter, null);
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="additionalConditions"></param>
        /// <returns></returns>
        public object BuildItem(string path, object parameter, IEnumerable<ICondition> additionalConditions)
        {
            int pos = path.LastIndexOf('/');
            string parent = path.Substring(0, pos);
            string child = path.Substring(pos + 1);
            AddInTreeNode node = this.addInTree.GetTreeNode(parent);

            return node.BuildChildItem(child, parameter, additionalConditions);
        }

        /// <summary>
        /// 生成对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="throwOnNotFound"></param>
        /// <returns></returns>
        public List<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound = true)
        {
            AddInTreeNode node = this.addInTree.GetTreeNode(path, throwOnNotFound);
            if (node == null)
                return new List<T>();
            else
                return node.BuildChildItems<T>(parameter);
        }

      
    }
}
