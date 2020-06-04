package com.perfect99.receiveUnicomDomain;


import java.util.Date;

public class HyzlChange {
    /// <summary>
    /// 序号
    /// </summary>
    private   int id ;

    /// <summary>
    /// 会员编号
    /// </summary>
    private  String hybh ;

    /// <summary>
    /// 会员姓名
    /// </summary>
    private  String hyxm ;

    /// <summary>
    /// sex
    /// </summary>
    private  String sex ;

    /// <summary>
    /// sfzh
    /// </summary>
    private  String sfzh ;

    /// <summary>
    /// cdate
    /// </summary>
    private Date cdate ;

    /// <summary>
    /// type
    /// </summary>
    private  int type ;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getHybh() {
        return hybh;
    }

    public void setHybh(String hybh) {
        this.hybh = hybh;
    }

    public String getHyxm() {
        return hyxm;
    }

    public void setHyxm(String hyxm) {
        this.hyxm = hyxm;
    }

    public String getSex() {
        return sex;
    }

    public void setSex(String sex) {
        this.sex = sex;
    }

    public String getSfzh() {
        return sfzh;
    }

    public void setSfzh(String sfzh) {
        this.sfzh = sfzh;
    }

    public Date getCdate() {
        return cdate;
    }

    public void setCdate(Date cdate) {
        this.cdate = cdate;
    }

    public int getType() {
        return type;
    }

    public void setType(int type) {
        this.type = type;
    }
}