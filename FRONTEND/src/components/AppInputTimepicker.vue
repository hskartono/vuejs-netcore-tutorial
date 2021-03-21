<template>
    <div>
        <div v-if="!nolabel">
            <b-form-group id="fieldset-is-received" boundary="viewport" v-bind="$attrs">
                <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
                <div v-if="isAllowEdit">
                    <b-input-group class="mb-3">
                        <b-input-group-prepend>
                            <b-form-timepicker
                                v-model="inputModelTime"
                                button-only
                                :hour12="false"
                                locale="en"
                                size="sm"
                                class="bg-transparent"
                                aria-controls="input-timepicker"
                                @input="setTime"
                            ></b-form-timepicker>
                        </b-input-group-prepend>
                        <b-form-input
                            id="example-input"
                            v-model="inputModelStr"
                            type="text"
                            placeholder="No datetime selected"
                            autocomplete="off"
                            size="sm"
                            readonly
                        ></b-form-input>
                    </b-input-group>
                    <b-form-invalid-feedback id="input-name-live-feedback" :state="state">
                        {{ errorMessage }}
                    </b-form-invalid-feedback>
                </div>
                <div v-else>
                    {{ inputModel }}
                </div>
            </b-form-group>
        </div>
        <div v-else>
            <div v-if="isAllowEdit">
                <b-input-group class="mb-3">
                    <b-input-group-prepend>
                        <b-form-timepicker
                            v-model="inputModelTime"
                            button-only
                            :hour12="false"
                            locale="en"
                            size="sm"
                            class="bg-transparent"
                            aria-controls="input-timepicker"
                            @input="setTime"
                        ></b-form-timepicker>
                    </b-input-group-prepend>
                    <b-form-input
                        id="example-input"
                        v-model="inputModelStr"
                        type="text"
                        placeholder="No datetime selected"
                        autocomplete="off"
                        size="sm"
                        readonly
                    ></b-form-input>
                </b-input-group>
                <b-form-invalid-feedback id="input-name-live-feedback" :state="state">
                    {{ errorMessage }}
                </b-form-invalid-feedback>
            </div>
            <div v-else>
                {{ inputModel }}
            </div>
        </div>
    </div>
</template>
<style lang="scss" scoped>
    /deep/.btn.btn-secondary, /deep/.btn.btn-secondary:active, /deep/.btn.btn-secondary:hover {
        background-color: transparent !important;
    }
    /deep/.form-control:disabled, .form-control[readonly] {
        background-color: transparent !important;
    }
    /deep/::placeholder {
        color: #b5b5c3 !important;
        background-color: transparent !important;
    }

    /deep/:-ms-input-placeholder {
        color: #b5b5c3 !important;
        background-color: transparent !important;
    }

    /deep/::-ms-input-placeholder {
        color: #b5b5c3 !important;
        background-color: transparent !important;
    }
</style>
<script>
import moment from 'moment';

export default {
    props: ["model", "errorMessage", "state", "suffix", "prefix", "isEditable", "change", "nolabel", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            inputModel:this.model,
            inputModelDate: '',
            inputModelTime: '',
            inputModelStr: '',
            prefixExist: (this.prefix ? true : false),
            suffixExist: (this.suffix ? true : false),
            value:'',
        }
    }, 
    methods: {
        setDate() {
            let inputModelDate = '1970-01-01';
            if (this.inputModelDate != '') {
                inputModelDate = this.inputModelDate;
            }
            let inputModelTime = '00:00';
            if (this.inputModelTime != '') {
                inputModelTime = this.inputModelTime;
            }
            this.inputModel = inputModelDate + ' ' + inputModelTime;
            let date = new Date(this.inputModel);
            if (date) {
                this.inputModelStr = moment(date).format('HH:mm');
            }
            this.$emit('update:model', this.inputModel);
            this.$emit('change')
        },
        setTime() {
            let inputModelDate = '1970-01-01';
            if (this.inputModelDate != '') {
                inputModelDate = this.inputModelDate;
            }
            let inputModelTime = '00:00';
            if (this.inputModelTime != '') {
                inputModelTime = this.inputModelTime;
            }
            this.inputModel = inputModelDate + ' ' + inputModelTime;
            let date = new Date(this.inputModel);
            if (date) {
                this.inputModelStr = moment(date).format('HH:mm');
            }
            this.$emit('update:model', this.inputModel);
            this.$emit('change')
        }
    },
    mounted() {
        console.log(this.state);
    },
    computed : {
        isAllowEdit : {
            get: function() {
                if (this.isEditable != undefined) return this.isEditable;
                else return true;
            }
        }
    },
    watch: {
        model(newValue) {
            let date = new Date(newValue);
            if (date != 'Invalid Date') {
                this.inputModelStr = moment(date).format('HH:mm');
                this.inputModelDate = moment(date).format('YYYY-MM-DD');
                this.inputModelTime = moment(date).format('HH:mm');
            }
            this.inputModel = newValue
        },
    }
}
</script>
<style scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
</style>