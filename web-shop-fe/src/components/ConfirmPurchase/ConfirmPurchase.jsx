import React, { useContext } from "react";
import { CartContext } from "../../store/cartContext";
import { Link, useNavigate } from "react-router-dom";
import buyerService from "../../services/buyerService";
import { PayPalButtons, PayPalScriptProvider } from "@paypal/react-paypal-js";
import classes from "./ConfirmPurchase.module.css";

const ConfirmPurchase = ({ address, comment }) => {
  const cartContext = useContext(CartContext);
  const navigator = useNavigate();

  const handleConfirmOrder = async () => {
    const items = cartContext.cartItems.map((item) => ({
      productId: item.id,
      productAmount: item.quantity,
    }));

    const createOrderValues = {
      items,
      deliveryAddress: cartContext.address.trim(),
      comment: cartContext.comment,
    };

    try {
      await buyerService.createOrder(createOrderValues);
      cartContext.clearCart();
      cartContext.setCartAddress("");
      cartContext.setCartComment("");
      navigator("/orders_buyer");
    } catch (error) {
      if (error.response) alert(error.response.data.Exception);
    }
  };

  const handlePayPalPayment = async (data, actions) => {
    const temp = cartContext.cartItems.map((item) => ({
      productId: item.id,
      productAmount: item.quantity,
    }));
    const price = await buyerService.getTotalPrice(temp);

    return actions.order.create({
      purchase_units: [
        {
          amount: {
            value: price.toFixed(2).toString(),
            currency_code: "USD",
          },
        },
      ],
    });
  };

  const handleApprovement = (data, actions) => {
    return actions.order.capture().then(async (details) => {
      const items = cartContext.cartItems.map((item) => ({
        productId: item.id,
        productAmount: item.quantity,
      }));

      const createOrderValues = {
        items,
        deliveryAddress: cartContext.address.trim(),
        comment: cartContext.comment,
      };

      try {
        await buyerService.createOrder(createOrderValues);
        cartContext.clearCart();
        cartContext.setCartAddress("");
        cartContext.setCartComment("");
        navigator("/orders_buyer");
      } catch (error) {
        if (error.response) alert(error.response.data.Exception);
      }
    });
  };

  return (
    <div className={classes.container}>
      <button onClick={handleConfirmOrder} className={classes.buttonPay}>
        Cash on arrival
      </button>
      <PayPalScriptProvider
        options={{ "client-id": process.env.REACT_APP_PAYPAL_ID }}
      >
        <PayPalButtons
          style={{ label: "checkout" }}
          createOrder={handlePayPalPayment}
          onApprove={handleApprovement}
        />
      </PayPalScriptProvider>
      <Link className={classes.link} to={"/cart"}>
        Back to cart
      </Link>
    </div>
  );
};

export default ConfirmPurchase;
