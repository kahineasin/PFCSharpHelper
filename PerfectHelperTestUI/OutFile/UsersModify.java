package com.perfect99.receiveUnicomDomain;


import java.util.Date;

public class UsersModify {
    private String userid;
    /// <summary>
    /// 姓名
    /// </summary>
public  String username ;

    /// <summary>
    /// 直销员证号
    /// </summary>
public  String sellerno ;

    /// <summary>
    /// 性别
    /// </summary>
public  String sex ;

    /// <summary>
    /// 身份证号
    /// </summary>
public  String sfz ;

    /// <summary>
    /// modificationTime
    /// </summary>
public  Date modificationTime ;

    /// <summary>
    /// 操作员
    /// </summary>
public  String modifier ;

    /// <summary>
    /// alertContent
    /// </summary>
public  String alertContent ;

    public  String getUsername (){
  return username;
}
public  void setUsername (String username){
  this.username=username;
}
public  String getSellerno (){
  return sellerno;
}
public  void setSellerno (String sellerno){
  this.sellerno=sellerno;
}
public  String getSex (){
  return sex;
}
public  void setSex (String sex){
  this.sex=sex;
}
public  String getSfz (){
  return sfz;
}
public  void setSfz (String sfz){
  this.sfz=sfz;
}
public  Date getModificationTime(){
  return modificationTime;
}
public  void setModificationTime(Date modificationTime){
  this.modificationTime=modificationTime;
}
public  String getModifier (){
  return modifier;
}
public  void setModifier (String modifier){
  this.modifier=modifier;
}
public  String getAlertContent (){
  return alertContent;
}
public  void setAlertContent (String alertContent){
  this.alertContent=alertContent;
}
public String getUserid() {
  return userid;
}
public void setUserid(String userid) {
  this.userid = userid;
}
}