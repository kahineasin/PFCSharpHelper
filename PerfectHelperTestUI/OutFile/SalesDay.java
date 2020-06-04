package com.perfect99.receiveUnicomDomain;

import java.math.BigDecimal;
import java.util.Date;
import java.util.UUID;

public class SalesDay {
    /// <summary>
    /// SalesDayId
    /// </summary>
	public  UUID SalesDayId ;
	
    /// <summary>
    /// SfNo
    /// </summary>
	public  String SfNo ;
	
    /// <summary>
    /// SfName
    /// </summary>
	public  String SfName ;
	
    /// <summary>
    /// 分公司编号
    /// </summary>
	public  String FgsNo ;
	
    /// <summary>
    /// FgsName
    /// </summary>
	public  String FgsName ;
	
    /// <summary>
    /// InvTypeNo
    /// </summary>
	public  String InvTypeNo ;
	
    /// <summary>
    /// InvTypeName
    /// </summary>
	public  String InvTypeName ;
	
    /// <summary>
    /// CDay
    /// </summary>
	public  Date CDay ;
	
    /// <summary>
    /// TotalMoney
    /// </summary>
	public  BigDecimal TotalMoney ;
	
    /// <summary>
    /// TotalPv
    /// </summary>
	public  int TotalPv ;
	
			public  UUID getSalesDayId (){
	  return SalesDayId;
	}
	public  void setSalesDayId (UUID SalesDayId){
	  this.SalesDayId=SalesDayId;
	}
	public  String getSfNo (){
	  return SfNo;
	}
	public  void setSfNo (String SfNo){
	  this.SfNo=SfNo;
	}
	public  String getSfName (){
	  return SfName;
	}
	public  void setSfName (String SfName){
	  this.SfName=SfName;
	}
	public  String getFgsNo (){
	  return FgsNo;
	}
	public  void setFgsNo (String FgsNo){
	  this.FgsNo=FgsNo;
	}
	public  String getFgsName (){
	  return FgsName;
	}
	public  void setFgsName (String FgsName){
	  this.FgsName=FgsName;
	}
	public  String getInvTypeNo (){
	  return InvTypeNo;
	}
	public  void setInvTypeNo (String InvTypeNo){
	  this.InvTypeNo=InvTypeNo;
	}
	public  String getInvTypeName (){
	  return InvTypeName;
	}
	public  void setInvTypeName (String InvTypeName){
	  this.InvTypeName=InvTypeName;
	}
	public  Date getCDay (){
	  return CDay;
	}
	public  void setCDay (Date CDay){
	  this.CDay=CDay;
	}
	public  BigDecimal getTotalMoney (){
	  return TotalMoney;
	}
	public  void setTotalMoney (BigDecimal TotalMoney){
	  this.TotalMoney=TotalMoney;
	}
	public  int getTotalPv (){
	  return TotalPv;
	}
	public  void setTotalPv (int TotalPv){
	  this.TotalPv=TotalPv;
	}
}