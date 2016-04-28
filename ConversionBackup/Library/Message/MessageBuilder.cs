﻿using System;
using System.Collections.Generic;
using System.Text;
public partial class MessageBuilder
{
    private string mPackage;
    private List<string> mKeys;
    private Dictionary<string, List<PackageField>> mCustoms = new Dictionary<string, List<PackageField>>();
    private Dictionary<string, List<PackageEnum>> mEnums = new Dictionary<string, List<PackageEnum>>();
    private Dictionary<string, List<PackageConst>> mConsts = new Dictionary<string, List<PackageConst>>();
    public void Transform(string configPath, string package, Dictionary<PROGRAM, ProgramConfig> programConfigs)
    {
        try {
            configPath = FileUtil.GetFullPath(configPath);
            Util.InitializeProgram(programConfigs);
            Util.ParseStructure(configPath, mCustoms, mEnums, null, null, null, mConsts);
            mPackage = package;
            mKeys = new List<string>(mCustoms.Keys);
            mKeys.Sort();
            var infos = Util.GetProgramInfos();
            Progress.Count = mCustoms.Count + mEnums.Count + mConsts.Count;
            Progress.Current = 0;
            foreach (var pair in mCustoms) {
                ++Progress.Current;
                Logger.info("正在转换类 {0}/{1} [{2}]", Progress.Current, Progress.Count, pair.Key);
                foreach (var info in infos.Values) {
                    if (!info.Create) continue;
                    info.CreateFile(pair.Key, info.GenerateMessage.Generate(pair.Key, mPackage, pair.Value, false));
                }
            }
            foreach (var pair in mEnums) {
                ++Progress.Current;
                Logger.info("正在转换枚举 {0}/{1} [{2}]", Progress.Current, Progress.Count, pair.Key);
                foreach (var info in infos.Values) {
                    if (!info.Create) continue;
                    info.CreateFile(pair.Key, info.GenerateEnum.Generate(pair.Key, mPackage, pair.Value));
                }
            }
            foreach (var pair in mConsts) {
                ++Progress.Current;
                Logger.info("正在转换常量 {0}/{1} [{2}]", Progress.Current, Progress.Count, pair.Key);
                foreach (var info in infos.Values) {
                    if (!info.Create) continue;
                    info.CreateFile(pair.Key, info.GenerateConst.Generate(pair.Key, mPackage, pair.Value));
                }
            }
            foreach (var info in infos.Values) {
                if (!info.Create) continue;
                info.CreateMessageManager.Invoke(this, null);
            }
        } catch (Exception ex) {
            Logger.error("转换消息出错 " + ex.ToString());
        }
    }
}