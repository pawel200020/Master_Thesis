package org.dbFrameworks.entities;

import jakarta.persistence.*;

import java.sql.Date;

@Entity
public class Orders {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long OrderId;
    private Date OrderDate;
    private String OrderDetails;
    private Double TotalCost;
    @ManyToOne
    @JoinColumn(name="ClientId")
    private Clients client;
    @ManyToOne
    @JoinColumn(name="EmployeeId")
    private Employees employee;
    @ManyToOne
    @JoinColumn(name="StoreId")
    private Stores store;

    public Employees getEmployee() {
        return employee;
    }

    public void setEmployee(Employees employee) {
        this.employee = employee;
    }

    public Long getOrderId() {
        return OrderId;
    }

    public void setOrderId(Long orderId) {
        OrderId = orderId;
    }

    public Date getOrderDate() {
        return OrderDate;
    }

    public void setOrderDate(Date orderDate) {
        OrderDate = orderDate;
    }

    public String getOrderDetails() {
        return OrderDetails;
    }

    public void setOrderDetails(String orderDetails) {
        OrderDetails = orderDetails;
    }

    public Double getTotalCost() {
        return TotalCost;
    }

    public void setTotalCost(Double totalCost) {
        TotalCost = totalCost;
    }

    public Clients getClient() {
        return client;
    }

    public void setClient(Clients client) {
        this.client = client;
    }

    public Stores getStore() {
        return store;
    }

    public void setStore(Stores store) {
        this.store = store;
    }
}
