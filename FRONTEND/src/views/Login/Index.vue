<template>
  <div>
    <b-container fluid>
        <b-row>
            <div class="front-page">
                <b-card>
                    <div class="text">
                        <div class="logo">
                            <img src="/img/logo.png" class="menu-logo" id="menu-logo" alt="menu logo" />
                        </div>
                        <div class="division">
                            Toyota PPO System
                        </div>
                    </div>
                    <div class="form-login pl-5 pr-5 d-none">
                        <b-form-input id="input-username" placeholder="Username" v-model="username" size="sm"></b-form-input>
                        <b-input type="password" id="input-password" placeholder="Password" class="mt-3" v-model="password" size="sm"></b-input>
                    </div>
                    <div class="action-button mt-4 mb-2 pl-5 pr-5">
                        <b-row>
                            <b-col>
                                <b-button class="btn-block" variant="danger" @click="login">Login</b-button>
                            </b-col>
                        </b-row>
                    </div>
                </b-card>
            </div>
        </b-row>
    </b-container>
  </div>
</template>
<style lang="scss" scoped>
   /deep/ .front-page {
        background: #fff url('/img/bg-yard-grayscaled.jpg') no-repeat center center / cover;
        height: 100%;
        left: 0;
        position: fixed;
        top: 0;
        width: 100%;
        z-index: 1000;

        .card {
            background-color: white;
            box-shadow: 0 1px 20px #000;
            left: 50%;
            min-width: 260px;
            max-width: 500px;
            position: absolute;
            text-align: center;
            top: 50%;
            transform: translate(-50%, -50%);
            width: 80%;
        }

        .card-body {
            padding-right: 0px;
            padding-left: 0px;
        }

        .text {
            padding: 30px 30px 20px 30px;

            .division {
                color: #555;
                font-size: 24px;
                font-weight: bold;
                margin-top: 20px;
                position: relative;

                &:before {
                    position: absolute;
                    top: -5px;
                    left: 50%;
                    transform: translateX(-50%);
                    width: 50px;
                    height: 1px;
                    background-color: rgba(100, 100, 100, 0.5);
                    content: "";
                }
            }

            img {
                display: block;
                margin: 0 auto;
                height: 100px;
            }

            .error-login {
                border-width:medium;
                font-size:medium;
                color: #CC0400;
                text-align: center;
                border:solid;
                border-color: red;
            }

            .error {
                color: #CC0400;
                text-align: left;
            }
        }

        .form-login {
            background: #BF0400;
            padding: 20px;
        }
        .action-button {
            .btn-danger {
                color: #fff;
                font-weight: 600;
                background-color: #BF0400;
                border-color: #BF0400;
            }
            .btn-danger:active, .btn-danger:focus, .btn-danger:hover {
                background-color: #a50602 !important;
                border-color: #a50602 !important;
            }
        }
    }
</style>
<script>
import axios from 'axios';

export default {
    components : {},
    name: 'Login',
    methods : {
    },
    data() {
      return {
        username : "",
        password : ""
      }
    },
    methods: {
        login() {
            this.$auth.loginWithRedirect();
        },
    }
}
</script>