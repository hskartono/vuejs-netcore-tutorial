<template>
    <div>
        <div v-if="useLabel">
            <b-form-group id="fieldset-is-received" boundary="viewport" v-bind="$attrs">
                <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
                <b-row>
                    <b-col cols="12">
                        <b-input-group class="mb-3">
                            <b-input-group-prepend>
                                <b-form-timepicker
                                    boundary="viewport"
                                    v-model="inputModelFromTime"
                                    button-only
                                    :hour12="false"
                                    locale="en"
                                    size="sm"
                                    class="bg-transparent"
                                    aria-controls="input-timepicker"
                                    @input="setTimeFrom"
                                ></b-form-timepicker>
                            </b-input-group-prepend>
                            <b-form-input
                                id="example-input"
                                v-model="inputModelFromStr"
                                type="text"
                                placeholder="No time selected"
                                autocomplete="off"
                                size="sm"
                                readonly
                            ></b-form-input>
                            <b-input-group-append v-if="inputModelFromStr">
                                <b-button size="sm" variant="outline-danger" @click="clearDateFrom"><b-icon-x /></b-button>
                            </b-input-group-append>
                        </b-input-group>
                    </b-col>
                    <b-col cols="12">
                        <b-input-group class="mb-3">
                            <b-input-group-prepend>
                                <b-form-timepicker
                                    boundary="viewport"
                                    v-model="inputModelToTime"
                                    button-only
                                    :hour12="false"
                                    locale="en-GB"
                                    size="sm"
                                    class="bg-transparent"
                                    aria-controls="input-timepicker"
                                    @input="setTimeTo"
                                ></b-form-timepicker>
                            </b-input-group-prepend>
                            <b-form-input
                                id="example-input"
                                v-model="inputModelToStr"
                                type="text"
                                placeholder="No time selected"
                                autocomplete="off"
                                size="sm"
                                readonly
                            ></b-form-input>
                            <b-input-group-append v-if="inputModelToStr">
                                <b-button size="sm" variant="outline-danger" @click="clearDateTo"><b-icon-x /></b-button>
                            </b-input-group-append>
                        </b-input-group>
                    </b-col>
                </b-row>
            </b-form-group>
        </div>
        <div v-else>
            <b-row class="position-static">
                <b-col cols="12" class="position-static" style="position:static">
                    <b-input-group class="mb-3 position-static" style="position:static">
                        <b-input-group-prepend class="position-static" style="position:static">
                            <b-form-datepicker
                                boundary="viewport"
                                v-model="inputModelFromDate"
                                button-only
                                locale="en-GB"
                                aria-controls="input-datepicker"
                                size="sm"
                                class="bg-transparent"
                                :max="inputModelTo"
                                @input="setDateFrom"
                            ></b-form-datepicker>
                            <b-form-timepicker
                                boundary="viewport"
                                v-model="inputModelFromTime"
                                button-only
                                :hour12="false"
                                locale="en-GB"
                                size="sm"
                                class="bg-transparent"
                                aria-controls="input-timepicker"
                                @input="setTimeFrom"
                            ></b-form-timepicker>
                        </b-input-group-prepend>
                        <b-form-input
                            id="example-input"
                            v-model="inputModelFromStr"
                            type="text"
                            placeholder="No datetime selected"
                            autocomplete="off"
                            size="sm"
                            readonly
                        ></b-form-input>
                        <b-input-group-append v-if="inputModelFromStr">
                            <b-button size="sm" variant="outline-danger" @click="clearDateFrom"><b-icon-x /></b-button>
                        </b-input-group-append>
                    </b-input-group>
                </b-col>
                <b-col cols="12" class="mb-3 position-static">
                    <b-input-group class="mb-3 position-static">
                        <b-input-group-prepend class="position-static">
                            <b-form-datepicker
                                boundary="viewport"
                                v-model="inputModelToDate"
                                button-only
                                locale="en-GB"
                                aria-controls="input-datepicker"
                                size="sm"
                                class="bg-transparent"
                                :min="inputModelFrom"
                                @input="setDateTo"
                            ></b-form-datepicker>
                            <b-form-timepicker
                                boundary="viewport"
                                v-model="inputModelToTime"
                                button-only
                                :hour12="false"
                                locale="en-GB"
                                size="sm"
                                class="bg-transparent"
                                aria-controls="input-timepicker"
                                @input="setTimeTo"
                            ></b-form-timepicker>
                        </b-input-group-prepend>
                        <b-form-input
                            id="example-input"
                            v-model="inputModelToStr"
                            type="text"
                            placeholder="No datetime selected"
                            autocomplete="off"
                            size="sm"
                            readonly
                        ></b-form-input>
                        <b-input-group-append v-if="inputModelToStr">
                            <b-button size="sm" variant="outline-danger" @click="clearDateTo"><b-icon-x /></b-button>
                        </b-input-group-append>
                    </b-input-group>
                </b-col>
            </b-row>
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
    /deep/.input-group-prepend, /deep/.input-group, /deep/.col-12, /deep/.row {
        position: static;
    }
    /deep/.form-group {
        margin-bottom: 0px;
    }
</style>
<script>
import moment from 'moment';

export default {
    props: ["modelFrom", "modelTo", "errorMessage", "state", "suffix", "prefix", "isEditable", "change", "nolabel", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            inputModelFrom: this.modelFrom,
            inputModelFromDate: '',
            inputModelFromTime: '',
            inputModelFromStr: '',
            inputModelTo: this.modelTo,
            inputModelToDate: '',
            inputModelToTime: '',
            inputModelToStr: '',
            value:''
        }
    }, 
    methods: {
        setDateFrom() {
            let inputModelFromDate = '1970-01-01';
            if (this.inputModelFromDate != '') {
                inputModelFromDate = this.inputModelFromDate;
            }
            let inputModelFromTime = '00:00';
            if (this.inputModelFromTime != '') {
                inputModelFromTime = this.inputModelFromTime;
            }
            this.inputModelFrom = inputModelFromDate + ' ' + inputModelFromTime;
            let date = new Date(this.inputModelFrom);
            if (date) {
                this.inputModelFromStr = moment(date).format('HH:mm');
            }
            this.$emit('update:modelFrom', this.inputModelFrom);
            this.$emit('change')
        },
        setTimeFrom() {
            let inputModelFromDate = '1970-01-01';
            if (this.inputModelFromDate != '') {
                inputModelFromDate = this.inputModelFromDate;
            }
            let inputModelFromTime = '00:00';
            if (this.inputModelFromTime != '') {
                inputModelFromTime = this.inputModelFromTime;
            }
            this.inputModelFrom = inputModelFromDate + ' ' + inputModelFromTime;
            let date = new Date(this.inputModelFrom);
            if (date) {
                this.inputModelFromStr = moment(date).format('HH:mm');
            }
            this.$emit('update:modelFrom', this.inputModelFrom);
            this.$emit('change')
        },
        setDateTo() {
            let inputModelToDate = '1970-01-01';
            if (this.inputModelToDate != '') {
                inputModelToDate = this.inputModelToDate;
            }
            let inputModelToTime = '00:00';
            if (this.inputModelToTime != '') {
                inputModelToTime = this.inputModelToTime;
            }
            this.inputModelTo = inputModelToDate + ' ' + inputModelToTime;
            let date = new Date(this.inputModelTo);
            if (date) {
                this.inputModelToStr = moment(date).format('DD/MM/YYYY HH:mm');
            }
            this.$emit('update:modelTo', this.inputModelTo);
            this.$emit('change')
        },
        setTimeTo() {
            let inputModelToDate = '1970-01-01';
            if (this.inputModelToDate != '') {
                inputModelToDate = this.inputModelToDate;
            }
            let inputModelToTime = '00:00';
            if (this.inputModelToTime != '') {
                inputModelToTime = this.inputModelToTime;
            }
            this.inputModelTo = inputModelToDate + ' ' + inputModelToTime;
            let date = new Date(this.inputModelTo);
            if (date) {
                this.inputModelToStr = moment(date).format('HH:mm');
            }
            this.$emit('update:modelTo', this.inputModelTo);
            this.$emit('change');
        },
        clearDateFrom() {
            this.inputModelFromStr = '';
            this.inputModelFromDate = '';
            this.inputModelFromTime = '';
            this.inputModelFrom = '';
            this.$emit('update:modelFrom', this.inputModelFrom);
            this.$emit('change');
        },
        clearDateTo() {
            this.inputModelToStr = '';
            this.inputModelToDate = '';
            this.inputModelToTime = '';
            this.inputModelTo = '';
            this.$emit('update:modelTo', this.inputModelTo);
            this.$emit('change');
        }
    },
    mounted() {
    },
    computed : {
    },
    watch: {
        modelFrom(newValue) {
            if (newValue == '') { this.clearDateFrom(); }
            let date = new Date(newValue);
            if (date != 'Invalid Date') {
                this.inputModelFromStr = moment(date).format('HH:mm');
                this.inputModelFromDate = moment(date).format('YYYY-MM-DD');
                this.inputModelFromTime = moment(date).format('HH:mm');
            }
            this.inputModelFrom = newValue
        },
        modelTo(newValue) {
            if (newValue == '') { this.clearDateTo(); }
            let date = new Date(newValue);
            if (date != 'Invalid Date') {
                this.inputModelToStr = moment(date).format('HH:mm');
                this.inputModelToDate = moment(date).format('YYYY-MM-DD');
                this.inputModelToTime = moment(date).format('HH:mm');
            }
            this.inputModelTo = newValue
        },
    },
    computed : {
        useLabel() {
            if (this.nolabel != null && this.nolabel != undefined && this.nolabel != '') {
                if (this.nolabel) return false;
            }
            return true;
        }
    }
}
</script>
<style scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
</style>