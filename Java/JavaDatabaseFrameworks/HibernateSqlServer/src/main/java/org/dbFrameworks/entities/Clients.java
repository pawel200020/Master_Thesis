package org.dbFrameworks.entities;

import jakarta.persistence.*;

import java.util.ArrayList;
import java.util.List;

@Entity
public class Clients {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long ClientId;
    private String FirstName;
    private String LastName;
    private String PhoneNumber;
    private String Address;
    private String Country;
    @OneToMany
    @JoinColumn(name = "ClientId")
    private List<Orders> Orders;
    public Long getClientId() {
        return ClientId;
    }

    public void setClientId(Long clientId) {
        ClientId = clientId;
    }

    public String getFirstName() {
        return FirstName;
    }

    public void setFirstName(String firstName) {
        FirstName = firstName;
    }

    public String getLastName() {
        return LastName;
    }

    public void setLastName(String lastName) {
        LastName = lastName;
    }

    public String getPhoneNumber() {
        return PhoneNumber;
    }

    public void setPhoneNumber(String phoneNumber) {
        PhoneNumber = phoneNumber;
    }

    public String getAddress() {
        return Address;
    }

    public void setAddress(String address) {
        Address = address;
    }

    public String getCountry() {
        return Country;
    }

    public void setCountry(String country) {
        Country = country;
    }

    @Override
    public String toString() {
        return "Client{" +
                "ClientId=" + ClientId +
                ", FirstName='" + FirstName + '\'' +
                ", LastName='" + LastName + '\'' +
                ", PhoneNumber='" + PhoneNumber + '\'' +
                ", Address='" + Address + '\'' +
                ", Country='" + Country + '\'' +
                '}';
    }

    public List<org.dbFrameworks.entities.Orders> getOrders() {
        return Orders;
    }
    public void setOrders(List<org.dbFrameworks.entities.Orders> orders) {
        Orders = orders;
    }
}
