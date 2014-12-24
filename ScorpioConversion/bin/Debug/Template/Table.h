#pragma once
#include <string>
#include <map>
using namespace::std;
#define __TableName_FILE_MD5_CODE = "__MD5";
class __TableName : public MT_TableBase {
    private: string fileName;
    private: map<int,__DataName*> m_dataArray;
    public: __TableName (string fileName) {
        this->fileName = fileName;
    }
    public: void Initialize() {
        m_dataArray.clear();
        ScorpioReader reader = ScorpioReader(TableUtil.GetBuffer(fileName));
        int iRow = reader.ReadInt32();                      //����
        int iColums = reader.ReadInt32();                   //����
        int iCodeNum = reader.ReadInt32();                  //�Զ���������       
        if (!(reader.ReadString() == __TableName_FILE_MD5_CODE))    //��֤�ļ�MD5(���ṹ�Ƿ�ı�)
            throw new Exception("�ļ�[" + fileName + "]�汾��֤ʧ��");
        int i,j,index;
        for (i = 0; i < iColums; ++i) {
            index = reader.ReadInt32();                     //��ȡ������
            reader.ReadInt32();                             //��ȡ���Ƿ�������
            if (index == TableUtil.CLASS_VALUE) {           //��������Զ�����
                reader.ReadString();                        //��ȡ������
                for (j = 0; j < reader.ReadInt32(); ++j){   //�Զ������ֶθ���(ȫ��ת�ɻ��������Ժ�)
                    reader.ReadInt32();                     //ȡ���ֶ�����
                }
            }
        }
        for (i = 0; i < iRow; ++i) {
            __DataName * pData = MT_Data_Active::Read(reader, fileName);
            if (Contains(pData->ID()))
                throw new Exception("�ļ�" + fileName + "���ظ��� ID : " + pData.ID());
            m_dataArray.put(pData->ID(),pData);
        }
        reader.Close();
    }
    public: bool Contains(int ID) {
        return (m_dataArray.find(ID) != m_dataArray.end());
    }
};