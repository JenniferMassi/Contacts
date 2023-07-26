using System;
using System.Collections.Generic;

public class Contact {

    public static Contact currentContact { get; set; }
    public static List<Contact> contacts { get; set; }
    public int id { get; set; }
    public string firstName { get; set; }
    public string middleName { get; set; }
    public string lastName { get; set; }
    public string nickName { get; set; }
    public string title { get; set; }
    public DateTime? birthday { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string street { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public int zip { get; set; }
    public string website { get; set; }
    public string notes { get; set; }
    public string picture { get; set; }
    public bool isFavorite { get; set; }
    public bool isActive { get; set; }

    public override string ToString() {
        return isFavorite ? $"{firstName} {lastName} ❤" : $"{firstName} {lastName}";
    }


}//end class


