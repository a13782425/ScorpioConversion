using System;
using System.Collections.Generic;
public class __TableName : MT_TableBase {
	const string FILE_MD5_CODE = "__MD5";
    private string fileName = "";
    private Dictionary<__Key, __DataName> m_dataArray = new Dictionary<__Key, __DataName>();
    public __TableName (string fileName) {
        this.fileName = fileName;
    }
    public void Initialize() {
        m_dataArray.Clear();
        ScorpioReader reader = new ScorpioReader(TableUtil.GetBuffer(fileName));
        int iRow = reader.ReadInt32();                      //����
        int iColums = reader.ReadInt32();                   //����
        int iCodeNum = reader.ReadInt32();                  //�Զ���������
        if (reader.ReadString() != FILE_MD5_CODE)           //��֤�ļ�MD5(���ṹ�Ƿ�ı�)
            throw new System.Exception("�ļ�[" + fileName + "]�汾��֤ʧ��");
        int i,j,index;
        for (i = 0; i < iColums; ++i) {
            index = reader.ReadInt32();                     //��ȡ������
            reader.ReadInt32();                             //��ȡ���Ƿ�������
            if (index == TableUtil.CLASS_VALUE) {           //��������Զ�����
                reader.ReadString();                        //��ȡ������
                for (j = 0; j < reader.ReadInt32(); ++j) {  //�Զ������ֶθ���(ȫ��ת�ɻ��������Ժ�)
                    reader.ReadInt32();                     //ȡ���ֶ�����
                }
            }
        }
        for (i = 0; i < iRow; ++i) {
            __DataName pData = __DataName.Read(reader, fileName);
            if (Contains(pData.ID()))
                throw new System.Exception("�ļ�[" + fileName + "]���ظ��� ID : " + pData.ID());
            m_dataArray.Add(pData.ID(), pData);
        }
        reader.Close();
    }
    public override bool Contains(__Key ID) {
        return m_dataArray.ContainsKey(ID);
    }
	public __DataName GetElement(__Key ID) {
		if (Contains(ID))
			return m_dataArray[ID];
        TableUtil.Warning("__DataName key is not exist " + ID);
		return null;
	}
	public override __DataName GetValue(__Key ID) {
        return GetElement(ID);
	}
	public override int Count() {
		return m_dataArray.Count;
	}
	public Dictionary<__Key, __DataName> Datas() {
		return m_dataArray;
	}
}